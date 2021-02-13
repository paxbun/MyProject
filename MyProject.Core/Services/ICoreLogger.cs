using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Services
{
    /// <summary>
    /// 로거 서비스
    /// </summary>
    public interface ICoreLogger
    {
        /// <summary>
        /// 새 액션이 들어왔다는 것을 표시합니다.
        /// </summary>
        /// <param name="request">액션</param>
        public Task TrackRequestAsync(object request, CancellationToken cancellationToken = default);

        /// <summary>
        /// 액션을 처리하던 도중, 전체적인 결과엔 영향을 주지는 않지만 기록해야하는 오류가 발생했다는 것을 알립니다.
        /// </summary>
        /// <param name="request">액션</param>
        /// <param name="intermediate">처리 중이었던 데이터</param>
        /// <param name="ex">발생한 예외</param>
        public Task ReportIgnorableErrorAsync(object request, object intermediate, Exception ex, CancellationToken cancellationToken = default);

        /// <summary>
        /// 액션이 성공했음을 알립니다.
        /// </summary>
        /// <param name="request">액션</param>
        /// <param name="response">액션 결과</param>
        public Task ReportSuccessAsync(object request, object response, CancellationToken cancellationToken = default);

        /// <summary>
        /// 액션이 실패했음을 알립니다.
        /// </summary>
        /// <param name="request">액션</param>
        /// <param name="ex">발생한 예외</param>
        public Task ReportErrorAsync(object request, Exception ex, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 로깅 시 특별한 직렬화가 필요한 경우 구현하는 인터페이스
    /// </summary>
    public interface ICoreLoggerFormatter<T>
    {
        /// <summary>
        /// 원래 직렬화할 객체 대신 직렬화할 객체를 반환합니다.
        /// </summary>
        /// <param name="obj">원래 직렬화하려고 했던 객체</param>
        /// <returns>실제로 직렬화될 객체</returns>
        public object Format(T obj);
    }

    public static class CoreLoggerExtensions
    {
        /// <summary>
        /// <c>Core</c>에 등록된 모든 <c>ICoreLoggerFormatter</c>를 서비스 목록에 추가합니다.
        /// </summary>
        public static IServiceCollection AddCoreLoggerFormatters(this IServiceCollection services)
        {
            var formatterTypes = CoreRequestHelpers.GetTypesWithGenericInterface(
                typeof(ICoreLoggerFormatter<>));

            foreach (var (formatterType, interfacetype) in formatterTypes)
            {
                services.AddTransient(interfacetype, formatterType);
            }

            return services;
        }
    }
}
