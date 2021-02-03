using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Behaviors
{
    /// <summary>
    /// 주어진 사용자가 자기에게 권한이 없는 작업을 수행하려고 했을 때 발생하는 예외
    /// </summary>
    public class AuthorizationFailedException : Exception
    {
        public AuthorizationFailedException()
        {
        }

        public AuthorizationFailedException(string message)
            : base(message)
        {
        }

        public AuthorizationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// request 정의가 잘못되었을 때 발생하는 예외
    /// </summary>
    public class InvalidRequestTypeException : Exception
    {
        public InvalidRequestTypeException()
        {
        }

        public InvalidRequestTypeException(string message)
            : base(message)
        {
        }

        public InvalidRequestTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    /// <summary>
    /// 주어진 request의 권한과 UserIdentity의 권한이 일치하는지 확인하는 pipeline behavior 
    /// </summary>
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (request is IIdentityHolder holder)
            {
                var forAttribute = (ForAttribute)Attribute.GetCustomAttribute(
                    request.GetType(), typeof(ForAttribute));
                if (forAttribute == null)
                    throw new InvalidRequestTypeException();

                var identity = holder.Identity;
                if (identity == null)
                {
                    if (!forAttribute.AllowAnonymous)
                        throw new AuthorizationFailedException();
                }
                else
                {
                    if (!forAttribute.Types.Contains(identity.Type))
                        throw new AuthorizationFailedException();
                }
            }
            else
                throw new InvalidRequestTypeException();
            return await next();
        }
    }
}
