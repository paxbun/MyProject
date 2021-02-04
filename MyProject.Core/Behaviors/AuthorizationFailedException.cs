using System;

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
}
