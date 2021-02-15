using MyProject.Core.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace MyProject.Infrastructure.Services
{
    public class ConsoleLogger : ICoreLogger
    {
        private struct IntermediateError
        {
            public object Intermediate { get; set; }
            public Exception Exception { get; set; }
        }

        private class RequestData
        {
            public EventId EventId { get; set; }

            public ILogger Logger { get; set; }

            public List<IntermediateError> Errors { get; set; }
        }

        private static readonly Func<object, object> _defaultFormatter = o => o;

        private static readonly object _eventIdLock = new();

        private static readonly Dictionary<object, RequestData> _requests = new();

        private static readonly Dictionary<Type, Func<object, object>> _formatters = new();

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private static int _eventId = 1;

        private static EventId GetNextEventId()
        {
            lock (_eventIdLock)
            {
                EventId rtn = new(_eventId);
                ++_eventId;
                return rtn;
            }
        }

        private static RequestData StopTrack(object request)
        {
            RequestData data;
            lock (_requests)
            {
                data = _requests[request];
                _requests.Remove(request);
            }
            return data;
        }

        private readonly ILoggerFactory _factory;

        private readonly IServiceProvider _provider;

        public ConsoleLogger(ILoggerFactory factory, IServiceProvider provider)
        {
            _factory = factory;
            _provider = provider;
        }

        private Func<object, object> GetFormatter(object obj)
        {
            Func<object, object> formatter = null;
            var objType = obj.GetType();

            lock (_formatters)
            {
                if (!_formatters.TryGetValue(objType, out formatter))
                {
                    var interfaceType = typeof(ICoreLoggerFormatter<>).MakeGenericType(objType);
                    var implementaion = _provider.GetService(interfaceType);

                    if (implementaion != null)
                    {
                        var implementationType = implementaion.GetType();
                        var param = Expression.Parameter(typeof(object));
                        var castedParam = Expression.TypeAs(param, objType);

                        var implementationConst = Expression.Constant(implementaion);
                        var implementationFormatMethod = implementationType.GetMethod("Format");
                        var implementationFormatCall = Expression.Call(
                            implementationConst, implementationFormatMethod, castedParam);

                        formatter = Expression.Lambda<Func<object, object>>(
                            implementationFormatCall, param).Compile();
                    }
                    else
                        formatter = _defaultFormatter;

                    _formatters.Add(objType, formatter);
                }
            }

            return formatter;
        }

        private string SerializeObject(object obj)
        {
            var objToSerialize = GetFormatter(obj)(obj);
            return JsonSerializer.Serialize(objToSerialize, _serializerOptions);
        }

        private ILogger CreateLogger(object request) => _factory.CreateLogger(request.GetType().Name);

        public async Task TrackRequestAsync(object request, CancellationToken cancellationToken)
        {
            EventId nextEventId = GetNextEventId();
            lock (_requests)
            {
                _requests.Add(request, new RequestData
                {
                    EventId = nextEventId,
                    Logger = CreateLogger(request),
                    Errors = new()
                });
            }
        }

        public async Task ReportIgnorableErrorAsync(object request, object intermediate, Exception ex, CancellationToken cancellationToken)
        {
            lock (_requests)
            {
                _requests[request].Errors.Add(new IntermediateError { Exception = ex, Intermediate = intermediate });
            }
        }

        public async Task ReportSuccessAsync(object request, object response, CancellationToken cancellationToken)
        {
            RequestData data = StopTrack(request);

            var logger = data.Logger;
            var eventId = data.EventId;

            logger.LogInformation(eventId, "Request: {0}", SerializeObject(request));
            foreach (var intermediateError in data.Errors)
            {
                logger.LogWarning(eventId, "Exception: {0}", intermediateError.Exception);
                logger.LogWarning(eventId, "Intermediate: {0}", intermediateError.Intermediate);
            }
            logger.LogInformation(eventId, "Response: {0}", SerializeObject(response));

        }

        public async Task ReportErrorAsync(object request, Exception ex, CancellationToken cancellationToken)
        {
            RequestData data = StopTrack(request);

            var logger = data.Logger;
            var eventId = data.EventId;

            logger.LogInformation(eventId, "Request: {0}", SerializeObject(request));
            foreach (var intermediateError in data.Errors)
            {
                logger.LogWarning(eventId, "Exception: {0}", intermediateError.Exception);
                logger.LogWarning(eventId, "Intermediate: {0}", intermediateError.Intermediate);
            }
            logger.LogError(eventId, "Exception: {0}", ex);
        }
    }
}

#pragma warning restore CS1998