using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Behaviors
{
    /// <summary>
    /// 주어진 request의 권한과 UserIdentity의 권한이 일치하는지 확인하는 pipeline behavior 
    /// </summary>
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is ICoreRequestBase coreRequest)
            {
                var forAttribute = (ForAttribute)Attribute.GetCustomAttribute(
                    coreRequest.GetType(), typeof(ForAttribute));
                if (forAttribute == null)
                    throw new InvalidRequestTypeException();

                var identity = coreRequest.Identity;
                if (identity == null)
                {
                    if (!forAttribute.AllowAnonymous)
                        throw new AuthorizationFailedException();
                }
                else
                {
                    if (!forAttribute.AllowAnonymous && !forAttribute.Types.Contains(identity.Type))
                        throw new AuthorizationFailedException();
                }
                return await next();
            }
            else
            {
                throw new InvalidRequestTypeException();
            }
        }
    }
}
