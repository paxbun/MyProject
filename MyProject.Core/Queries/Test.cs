using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Queries
{
    [For(UserType.Administrator)]
    public record TestQuery : ICoreRequest<TestErrorReason, TestResultView>
    {
        public UserIdentity Identity { get; set; }

        public int Argument1 { get; set; }

        public int Argument2 { get; set; }
    }

    public enum TestErrorReason
    {
        UnknownError = 0,
        ValueIsNegative = 1,
        OutOfBound = 2,
    }

    public class TestQueryHandler : ICoreRequestHandler<TestQuery, TestErrorReason, TestResultView>
    {
        public async Task<Result<TestErrorReason, TestResultView>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            int result = request.Argument1 + request.Argument2;

            if (result < 0)
                return Result<TestErrorReason, TestResultView>.MakeFailure(TestErrorReason.ValueIsNegative);

            if (result > 20)
                return Result<TestErrorReason, TestResultView>.MakeFailure(TestErrorReason.OutOfBound);

            if (result == 13)
                throw new ArgumentException();

            return Result<TestErrorReason, TestResultView>.MakeSuccess(new TestResultView
            {
                Result = result
            });
        }
    }
}
