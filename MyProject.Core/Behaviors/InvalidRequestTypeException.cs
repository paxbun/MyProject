using System;

namespace MyProject.Core.Behaviors
{
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
}
