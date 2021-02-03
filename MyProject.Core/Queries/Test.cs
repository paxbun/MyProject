using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Queries
{
    [For(UserType.Administrator)]
    public record TestQuery : ICoreRequest<TestErrorResult, TestResultView>
    {
        public UserIdentity Identity { get; set; }

        public int Argument1 { get; set; }

        public int Argument2 { get; set; }
    }

    public enum TestErrorResult
    {
        UnknownError = 0,
        ValueIsNegative = 1,
        OutOfBound = 2,
    }

    public class TestQueryHandler : ICoreRequestHandler<TestQuery, TestErrorResult, TestResultView>
    {
        public async Task<Result<TestErrorResult, TestResultView>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            int result = request.Argument1 + request.Argument2;

            if (result < 0)
                return Result<TestErrorResult, TestResultView>.MakeFailure(TestErrorResult.ValueIsNegative);

            if (result > 20)
                return Result<TestErrorResult, TestResultView>.MakeFailure(TestErrorResult.OutOfBound);

            if (result == 13)
                throw new ArgumentException();

            return Result<TestErrorResult, TestResultView>.MakeSuccess(new TestResultView
            {
                Result = result
            });
        }
    }
}
