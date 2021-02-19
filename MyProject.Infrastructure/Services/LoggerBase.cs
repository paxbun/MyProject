using MyProject.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Infrastructure.Services
{
    public abstract class LoggerBase : ICoreLogger
    {
        protected struct IntermediateError
        {
            public object Intermediate { get; set; }
            public Exception Exception { get; set; }
        }

        protected class RequestData
        {
            public object Request { get; set; }
            public int EventId { get; set; }
            public List<IntermediateError> Errors { get; set; }
        }

        private static readonly Func<object, object> _defaultFormatter = o => o;

        private static readonly object _eventIdLock = new();

        private static readonly Dictionary<object, RequestData> _requests = new();

        private static readonly Dictionary<Type, Func<object, object>> _formatters = new();

        protected static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };

        private static int _eventId = 1;

        private static int GetNextEventId()
        {
            lock (_eventIdLock)
            {
                int rtn = _eventId;
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

        private readonly IServiceProvider _provider;

        protected LoggerBase(IServiceProvider provider) => _provider = provider;

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

                    if (implementaion is not null)
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

        protected string SerializeObject(object obj)
        {
            var objToSerialize = GetFormatter(obj)(obj);
            return JsonSerializer.Serialize(objToSerialize, _serializerOptions);
        }

        public async Task TrackRequestAsync(object request, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                int nextEventId = GetNextEventId();
                lock (_requests)
                {
                    _requests.Add(request, new RequestData
                    {
                        Request = request,
                        EventId = nextEventId,
                        Errors = new()
                    });
                }
            }, cancellationToken);
        }

        public async Task ReportIgnorableErrorAsync(object request, object intermediate, Exception ex, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                lock (_requests)
                {
                    _requests[request].Errors.Add(new IntermediateError { Exception = ex, Intermediate = intermediate });
                }
            }, cancellationToken);
        }

        public async Task ReportSuccessAsync(object request, object response, CancellationToken cancellationToken)
        {
            RequestData data = StopTrack(request);
            await ReportSuccessAsync(data, response, cancellationToken);
        }

        public async Task ReportErrorAsync(object request, Exception ex, CancellationToken cancellationToken)
        {
            RequestData data = StopTrack(request);
            await ReportErrorAsync(data, ex, cancellationToken);
        }

        protected abstract Task ReportSuccessAsync(RequestData data, object response, CancellationToken cancellationToken);

        protected abstract Task ReportErrorAsync(RequestData data, Exception ex, CancellationToken cancellationToken);
    }
}
