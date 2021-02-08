using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Behaviors
{
    /// <summary>
    /// 주어진 request에 대한 정보와 발생한 예외를 자동으로 기록하는 pipeline behavior
    /// </summary>
    public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILoggerFactory _factory;

        private static readonly object _eventIdLock = new();
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

        public LogBehavior(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICoreRequestBase<TResponse> coreRequest)
            {
                ILogger logger = _factory.CreateLogger(request.GetType().Name);
                EventId eventId = GetNextEventId();

                try
                {
                    var response = await next();
                    logger.LogInformation(eventId, "Request: {0}", coreRequest);
                    logger.LogInformation(eventId, "Response: {0}", response);
                    return response;
                }
                catch (Exception ex)
                {
                    logger.LogError(eventId, "Request: {0}", coreRequest);
                    logger.LogError(eventId, "Exception: {0}", ex);
                    return coreRequest.MakeDefaultFailure();
                }
            }
            else
            {
                throw new InvalidRequestTypeException();
            }
        }
    }
}
