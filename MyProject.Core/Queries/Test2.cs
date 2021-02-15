using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Queries
{
    [For(AllowAnonymous = true)]
    public class Test2Query : CoreRequest<Test2Error, Test2ResultView>
    {
        public int Argument1 { get; set; }

        public int Argument2 { get; set; }
    }

    public enum Test2ErrorType
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

    public struct Test2Error
    {
        public Test2ErrorType Type { get; set; }

        public int Data { get; set; }
    }

    public class Test2QueryHandler : ICoreRequestHandler<Test2Query, Test2Error, Test2ResultView>
    {
        public async Task<Result<Test2Error, Test2ResultView>> Handle(Test2Query request, CancellationToken cancellationToken)
        {
            int result = request.Argument1 + request.Argument2;

            if (result < 0)
                return request.MakeFailure(new Test2Error { Type = Test2ErrorType.ValueIsNegative });

            if (result > 20)
                return request.MakeFailure(new Test2Error { Type = Test2ErrorType.OutOfBound, Data = result });

            // 테스트용
            if (result == 13)
                throw new ArgumentException();

            return request.MakeSuccess(new Test2ResultView
            {
                Result = result
            });
        }
    }
}
