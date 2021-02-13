using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Queries
{
    [For(UserType.Administrator)]
    public class TestQuery : CoreRequest<TestError, TestResultView>
    {
        public int Argument1 { get; set; }

        public int Argument2 { get; set; }
    }

    public enum TestError
    {
        UnknownError,

        /// <summary>
        /// 결과가 음수
        /// </summary>
        [Display("결과가 음수입니다.", Locale = "ko-KR")]
        [Display("The result is negative.", Locale = "en-US")]
        ValueIsNegative,

        /// <summary>
        /// 결과가 20 초과
        /// </summary>
        [Display("결과가 20을 초과합니다.")]
        OutOfBound,
    }

    public class TestQueryHandler : ICoreRequestHandler<TestQuery, TestError, TestResultView>
    {
        public async Task<Result<TestError, TestResultView>> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            int result = request.Argument1 + request.Argument2;

            if (result < 0)
                return request.MakeFailure(TestError.ValueIsNegative);

            if (result > 20)
                return request.MakeFailure(TestError.OutOfBound);

            // 테스트용
            if (result == 13)
                throw new ArgumentException();

            return request.MakeSuccess(new TestResultView
            {
                Result = result
            });
        }
    }
}
