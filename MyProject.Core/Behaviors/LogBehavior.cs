using MediatR;
using MyProject.Core.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Behaviors
{
    /// <summary>
    /// 주어진 request에 대한 정보와 발생한 예외를 자동으로 기록하는 pipeline behavior
    /// </summary>
    public class LogBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ICoreLogger _logger;
        public LogBehavior(ICoreLogger logger) => _logger = logger;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICoreRequestBase<TResponse> coreRequest)
            {
                var doNotLog = Attribute.GetCustomAttribute(request.GetType(), typeof(DoNotLogAttribute), false) != null;
                if (doNotLog)
                {
                    try
                    {
                        return await next();
                    }
                    catch (Exception)
                    {
                        return coreRequest.MakeDefaultFailure();
                    }
                }
                else
                {
                    await _logger.TrackRequestAsync(coreRequest, cancellationToken);
                    try
                    {
                        var response = await next();
                        await _logger.ReportSuccessAsync(request, response, cancellationToken);
                        return response;
                    }
                    catch (Exception ex)
                    {
                        await _logger.ReportErrorAsync(request, ex, cancellationToken);
                        return coreRequest.MakeDefaultFailure();
                    }
                }
            }
            else
            {
                throw new InvalidRequestTypeException();
            }
        }
    }
}
