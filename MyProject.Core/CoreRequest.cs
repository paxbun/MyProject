using MediatR;
using MyProject.Core.ViewModels;
using MyProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProject.Core
{
    public interface ICoreRequestBase
    {
        /// <summary>
        /// 현재 로그인 중인 사용자 정보
        /// </summary>
        public UserIdentity Identity { get; set; }
    }

    public interface ICoreRequestBase<TResult> : ICoreRequestBase, IRequest<TResult>
    {
        /// <summary>
        /// 액션이 처리하지 않은 예외가 발생했을 때 반환해야하는 <c>Result</c> 또는 <c>DataResult</c> 객체를 반환하는 함수
        /// </summary>
        public TResult MakeDefaultFailure();
    }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public abstract class CoreRequest<TError, TResultData>
        : ICoreRequestBase<Result<TError, TResultData>>
        where TError : struct
        where TResultData : class
    {
        public UserIdentity Identity { get; set; }

        public Result<TError, TResultData> MakeSuccess(TResultData data = default) => Result<TError, TResultData>.MakeSuccess(data);

        public Result<TError, TResultData> MakeFailure(TError error = default) => Result<TError, TResultData>.MakeFailure(error);

        public Result<TError, TResultData> MakeDefaultFailure() => MakeFailure();
    }

    public interface ICoreRequestHandler<TCommand, TError, TResultData>
        : IRequestHandler<TCommand, Result<TError, TResultData>>
        where TCommand : CoreRequest<TError, TResultData>
        where TError : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">전체적인 연산이 실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TError">각각의 연산이 실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public abstract class CoreBatchRequest<TBatchError, TError, TResultData>
        : ICoreRequestBase<BatchResult<TBatchError, TError, TResultData>>
        where TBatchError : struct
        where TError : struct
        where TResultData : class
    {
        public UserIdentity Identity { get; set; }

        public List<Result<TError, TResultData>> MakeResultList() => new();

        public BatchResult<TBatchError, TError, TResultData> MakeBatchSuccess(IEnumerable<Result<TError, TResultData>> enumerable = default)
        {
            return BatchResult<TBatchError, TError, TResultData>.MakeSuccess(enumerable);
        }

        public BatchResult<TBatchError, TError, TResultData> MakeBatchFailure(TBatchError error = default)
        {
            return BatchResult<TBatchError, TError, TResultData>.MakeFailure(error);
        }

        public Result<TError, TResultData> MakeSuccess(TResultData data = default) => Result<TError, TResultData>.MakeSuccess(data);

        public Result<TError, TResultData> MakeFailure(TError error = default) => Result<TError, TResultData>.MakeFailure(error);

        public BatchResult<TBatchError, TError, TResultData> MakeDefaultFailure() => MakeBatchFailure();
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchError, TError, TResultData>
        : IRequestHandler<TCommand, BatchResult<TBatchError, TError, TResultData>>
        where TCommand : CoreBatchRequest<TBatchError, TError, TResultData>
        where TBatchError : struct
        where TError : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    public abstract class CoreRequest<TError>
        : ICoreRequestBase<Result<TError>>
        where TError : struct
    {
        public UserIdentity Identity { get; set; }

        public Result<TError> MakeSuccess() => Result<TError>.MakeSuccess();

        public Result<TError> MakeFailure(TError error = default) => Result<TError>.MakeFailure(error);

        public Result<TError> MakeDefaultFailure() => MakeFailure();
    }

    public interface ICoreRequestHandler<TCommand, TError>
        : IRequestHandler<TCommand, Result<TError>>
        where TCommand : CoreRequest<TError>
        where TError : struct
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">전체적인 연산이 실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TError">각각의 연산이 실패시 이유를 나타내는 열거형</typeparam>
    public abstract class CoreBatchRequest<TBatchError, TError>
        : ICoreRequestBase<BatchResult<TBatchError, TError>>
        where TBatchError : struct
        where TError : struct
    {
        public UserIdentity Identity { get; set; }

        public List<Result<TError>> MakeResultList() => new();

        public BatchResult<TBatchError, TError> MakeBatchSuccess(IEnumerable<Result<TError>> enumerable = default)
        {
            return BatchResult<TBatchError, TError>.MakeSuccess(enumerable);
        }

        public BatchResult<TBatchError, TError> MakeBatchFailure(TBatchError error = default)
        {
            return BatchResult<TBatchError, TError>.MakeFailure(error);
        }

        public Result<TError> MakeSuccess() => Result<TError>.MakeSuccess();

        public Result<TError> MakeFailure(TError error = default) => Result<TError>.MakeFailure(error);

        public BatchResult<TBatchError, TError> MakeDefaultFailure() => MakeBatchFailure();
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchError, TError>
        : IRequestHandler<TCommand, BatchResult<TBatchError, TError>>
        where TCommand : CoreBatchRequest<TBatchError, TError>
        where TBatchError : struct
        where TError : struct
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public abstract class CoreDataRequest<TResultData>
        : ICoreRequestBase<DataResult<TResultData>>
        where TResultData : class
    {
        public UserIdentity Identity { get; set; }

        public DataResult<TResultData> MakeSuccess(TResultData data = default) => DataResult<TResultData>.MakeSuccess(data);

        public DataResult<TResultData> MakeFailure() => DataResult<TResultData>.MakeFailure();

        public DataResult<TResultData> MakeDefaultFailure() => MakeFailure();
    }

    public interface ICoreDataRequestHandler<TCommand, TResultData>
        : IRequestHandler<TCommand, DataResult<TResultData>>
        where TCommand : CoreDataRequest<TResultData>
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">전체적인 연산이 실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public abstract class CoreBatchDataRequest<TBatchError, TResultData>
        : ICoreRequestBase<BatchDataResult<TBatchError, TResultData>>
        where TBatchError : struct
        where TResultData : class
    {
        public UserIdentity Identity { get; set; }

        public List<DataResult<TResultData>> MakeResultList() => new();

        public BatchDataResult<TBatchError, TResultData> MakeBatchSuccess(IEnumerable<DataResult<TResultData>> enumerable = default)
        {
            return BatchDataResult<TBatchError, TResultData>.MakeSuccess(enumerable);
        }

        public BatchDataResult<TBatchError, TResultData> MakeBatchFailure(TBatchError error = default)
        {
            return BatchDataResult<TBatchError, TResultData>.MakeFailure(error);
        }

        public DataResult<TResultData> MakeSuccess(TResultData data = default) => DataResult<TResultData>.MakeSuccess(data);

        public DataResult<TResultData> MakeFailure() => DataResult<TResultData>.MakeFailure();

        public BatchDataResult<TBatchError, TResultData> MakeDefaultFailure() => MakeBatchFailure();
    }

    public interface ICoreBatchDataRequestHandler<TCommand, TBatchError, TResultData>
        : IRequestHandler<TCommand, BatchDataResult<TBatchError, TResultData>>
        where TCommand : CoreBatchDataRequest<TBatchError, TResultData>
        where TBatchError : struct
        where TResultData : class
    { }

    public struct TypeInterfacePair
    {
        public Type Type { get; set; }
        public Type InterfaceType { get; set; }

        public void Deconstruct(out Type type, out Type interfaceType)
        {
            type = Type;
            interfaceType = InterfaceType;
        }
    }

    public static class CoreRequestHelpers
    {
        public static IEnumerable<TypeInterfacePair> GetTypesWithGenericInterface(Type genericInterface)
        {
            if (!genericInterface.IsInterface || !genericInterface.IsGenericType)
                throw new ArgumentException(nameof(genericInterface));

            var coreAssembly = typeof(CoreRequestHelpers).Assembly;
            foreach (var type in coreAssembly.GetTypes())
            {
                if (type.IsAbstract || type.IsInterface || !type.IsClass)
                    continue;

                var interfaces = type.GetInterfaces();
                var @interface = interfaces.Where(
                    i => i.IsGenericType
                        && i.GetGenericTypeDefinition() == genericInterface)
                    .FirstOrDefault();

                if (@interface != null)
                {
                    yield return new TypeInterfacePair
                    {
                        Type = type,
                        InterfaceType = @interface
                    };
                }
            }
        }
    }

    /// <summary>
    /// request를 사용할 수 있는 사용자를 정의하는 attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ForAttribute : Attribute
    {
        /// <summary>
        /// 익명 사용자가 사용할 수 있는가
        /// </summary>
        public bool AllowAnonymous { get; set; } = false;

        /// <summary>
        /// 이 request를 보낼 수 있는 사용자 종류 목록
        /// </summary>
        public UserType[] Types { get; }

        public ForAttribute(params UserType[] types) => Types = types;
    }
}
