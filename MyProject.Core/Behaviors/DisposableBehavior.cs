using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Behaviors
{
    public class DisposableBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            if (request is IDisposable disposable)
            {
                disposable.Dispose();
            }
            return response;
        }
    }
}
