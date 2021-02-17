using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using MyProject.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace MyProject.Infrastructure.Services
{
    public class AzureTableStorageLogger : LoggerBase, ICoreLogger
    {
        private class LogExceptionItem
        {
            public LogExceptionItem() { }
            public LogExceptionItem(object request, Exception ex)
            {
                var stackTraceRegex = new Regex($"at ({request.GetType().FullName}.+) in (.+\\.cs):line (\\d+)");
                var match = stackTraceRegex.Match(ex.StackTrace);

                Name = ex.GetType().Name;
                Method = match.Groups[1].Value;
                Filename = match.Groups[2].Value;
                Line = int.Parse(match.Groups[3].Value);
            }

            public string Name { get; set; }
            public string Method { get; set; }
            public string Filename { get; set; }
            public int Line { get; set; }
        }

        private class LogErrorItem
        {
            public string Intermediate { get; set; }
            public LogExceptionItem Exception { get; set; }
        }

        private class LogItem : ITableEntity
        {
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset Timestamp { get; set; }
            public string ETag { get; set; }
            public string Request { get; set; }
            public IList<LogErrorItem> IntermediateErrors { get; set; }
            public bool Success { get; set; }
            public string Response { get; set; }
            public LogExceptionItem Error { get; set; }

            public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
            {
                PartitionKey = properties[nameof(PartitionKey)].StringValue;
                RowKey = properties[nameof(RowKey)].StringValue;
                Timestamp = properties[nameof(Timestamp)].DateTimeOffsetValue ?? default;
                Request = properties[nameof(Request)].StringValue;
                IntermediateErrors = JsonSerializer.Deserialize<List<LogErrorItem>>(properties[nameof(IntermediateErrors)].StringValue);
                Success = properties[nameof(Success)].BooleanValue ?? true;
                Response = properties[nameof(Response)].StringValue;
                Error = JsonSerializer.Deserialize<LogExceptionItem>(properties[nameof(Error)].StringValue);
            }

            public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
            {
                return new Dictionary<string, EntityProperty>
                {
                    { nameof(PartitionKey), new(PartitionKey) },
                    { nameof(RowKey), new(RowKey) },
                    { nameof(Timestamp), new(Timestamp) },
                    { nameof(Request), new(Request) },
                    { nameof(IntermediateErrors), new(JsonSerializer.Serialize(IntermediateErrors)) },
                    { nameof(Success), new(Success) },
                    { nameof(Response), new(Response) },
                    { nameof(Error), new(JsonSerializer.Serialize(Error)) },
                };
            }
        }

        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudTableClient _tableClient;
        private readonly string _tableNamePrefix;
        private readonly int _queueCount;
        private readonly object _batchOperationLock = new();
        private int _eventId = 0;
        private Dictionary<string, TableBatchOperation> _batchOperations = new();
        private DateTime _batchOperationDate;

        public AzureTableStorageLogger(IConfiguration configuration, IServiceProvider provider)
            : base(provider)
        {
            _storageAccount = CloudStorageAccount.Parse(configuration["StorageConnectionString"]);
            _tableClient = _storageAccount.CreateCloudTableClient();
            _tableNamePrefix = configuration["StorageLogTableNamePrefix"];
            _queueCount = int.Parse(configuration["StorageLogQueueCount"]);
            if (_queueCount <= 0)
                throw new FormatException();
            _batchOperationDate = DateTimeOffset.Now.Date;
        }

        private void Log(LogItem logItem)
        {
            Task.Run(() =>
            {
                lock (_batchOperationLock)
                {
                    var now = DateTimeOffset.Now;
                    var nowString = now.ToString("HHmmssfffffff", System.Globalization.CultureInfo.InvariantCulture);
                    logItem.RowKey = nowString + _eventId.ToString();
                    logItem.Timestamp = now;
                    _eventId = (_eventId + 1) % 10;

                    var operation = TableOperation.Insert(logItem);
                    var operationDate = now.Date;

                    if (_batchOperationDate != operationDate)
                    {
                        CloudTable table = _tableClient.GetTableReference(
                            _tableNamePrefix + _batchOperationDate.ToString("yyyyMMdd"));
                        table.CreateIfNotExists();
                        foreach (var (_, batchOperation) in _batchOperations)
                        {
                            if (batchOperation.Count != 0)
                                table.ExecuteBatch(batchOperation);
                        }
                        _batchOperations = new();
                        _batchOperationDate = operationDate;
                    }

                    {
                        var batchOperation = _batchOperations.GetValueOrDefault(logItem.PartitionKey);
                        if (batchOperation == null)
                        {
                            batchOperation = new();
                            _batchOperations[logItem.PartitionKey] = batchOperation;
                        }

                        batchOperation.Add(operation);
                        if (batchOperation.Count >= _queueCount)
                        {
                            _batchOperations[logItem.PartitionKey] = new();
                            CloudTable table = _tableClient.GetTableReference(
                                _tableNamePrefix + _batchOperationDate.ToString("yyyyMMdd"));
                            table.CreateIfNotExists();
                            table.ExecuteBatch(batchOperation);
                        }
                    }
                }
            });
        }

        protected override async Task ReportSuccessAsync(RequestData data, object response, CancellationToken cancellationToken)
        {
            Log(new LogItem
            {
                PartitionKey = data.Request.GetType().Name,
                ETag = "*",
                Request = SerializeObject(data.Request),
                IntermediateErrors = data.Errors.Select(
                    error => new LogErrorItem
                    {
                        Intermediate = SerializeObject(error.Intermediate),
                        Exception = new LogExceptionItem(data.Request, error.Exception)
                    }).ToArray(),
                Success = true,
                Response = SerializeObject(response)
            });
        }

        protected override async Task ReportErrorAsync(RequestData data, Exception ex, CancellationToken cancellationToken)
        {
            Log(new LogItem
            {
                PartitionKey = data.Request.GetType().Name,
                ETag = "*",
                Request = SerializeObject(data.Request),
                IntermediateErrors = data.Errors.Select(
                    error => new LogErrorItem
                    {
                        Intermediate = SerializeObject(error.Intermediate),
                        Exception = new LogExceptionItem(data.Request, error.Exception)
                    }).ToArray(),
                Success = false,
                Response = null,
                Error = new LogExceptionItem(data.Request, ex)
            });
        }
    }
}

#pragma warning restore CS1998