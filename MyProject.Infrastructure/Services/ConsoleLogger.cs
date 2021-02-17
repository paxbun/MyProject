using Microsoft.Extensions.Logging;
using MyProject.Core.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS1998

namespace MyProject.Infrastructure.Services
{
    public class ConsoleLogger : LoggerBase, ICoreLogger
    {
        private readonly ILoggerFactory _factory;
        public ConsoleLogger(ILoggerFactory factory, IServiceProvider provider) : base(provider)
            => _factory = factory;

        private ILogger CreateLogger(object request) => _factory.CreateLogger(request.GetType().Name);

        protected override async Task ReportSuccessAsync(RequestData data, object response, CancellationToken cancellationToken)
        {
            var request = data.Request;
            var logger = CreateLogger(request);
            var eventId = data.EventId;

            logger.LogInformation(eventId, "Request: {0}", SerializeObject(request));
            foreach (var intermediateError in data.Errors)
            {
                logger.LogWarning(eventId, "Exception: {0}", intermediateError.Exception);
                logger.LogWarning(eventId, "Intermediate: {0}", intermediateError.Intermediate);
            }
            logger.LogInformation(eventId, "Response: {0}", SerializeObject(response));

        }

        protected override async Task ReportErrorAsync(RequestData data, Exception ex, CancellationToken cancellationToken)
        {
            var request = data.Request;
            var logger = CreateLogger(request);
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