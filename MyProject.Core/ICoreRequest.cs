using MyProject.Core.ViewModels;
using MyProject.Models;
using MediatR;
using System;
using System.Collections.Generic;

namespace MyProject.Core
{
    public interface ICoreRequestBase
    {
        /// <summary>
        /// 현재 로그인 중인 사용자 정보
        /// </summary>
        public UserIdentity Identity { get; set; }

        /// <summary>
        /// 액션이 처리하지 않은 예외가 발생했을 때 반환해야하는 <c>Result</c> 또는 <c>DataResult</c> 객체를 반환하는 함수
        /// </summary>
        public object MakeDefaultFailure();
    }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreRequest<TError, TResultData>
        : IRequest<Result<TError, TResultData>>, ICoreRequestBase
        where TError : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return Result<TError, TResultData>.MakeFailure();
        }
    }

    public interface ICoreRequestHandler<TCommand, TError, TResultData>
        : IRequestHandler<TCommand, Result<TError, TResultData>>
        where TCommand : ICoreRequest<TError, TResultData>
        where TError : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchRequest<TBatchError, TError, TResultData>
        : IRequest<BatchResult<TBatchError, TError, TResultData>>, ICoreRequestBase
        where TBatchError : struct
        where TError : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchResult<TBatchError, TError, TResultData>.MakeFailure();
        }
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchError, TError, TResultData>
        : IRequestHandler<TCommand, BatchResult<TBatchError, TError, TResultData>>
        where TCommand : ICoreBatchRequest<TBatchError, TError, TResultData>
        where TBatchError : struct
        where TError : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    public interface ICoreRequest<TError>
        : IRequest<Result<TError>>, ICoreRequestBase
        where TError : struct
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return Result<TError>.MakeFailure();
        }
    }

    public interface ICoreRequestHandler<TCommand, TError>
        : IRequestHandler<TCommand, Result<TError>>
        where TCommand : ICoreRequest<TError>
        where TError : struct
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchRequest<TBatchError, TError>
        : IRequest<BatchResult<TBatchError, TError>>, ICoreRequestBase
        where TBatchError : struct
        where TError : struct
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchResult<TBatchError, TError>.MakeFailure();
        }
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchError, TError>
        : IRequestHandler<TCommand, BatchResult<TBatchError, TError>>
        where TCommand : ICoreBatchRequest<TBatchError, TError>
        where TBatchError : struct
        where TError : struct
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreDataRequest<TResultData>
        : IRequest<DataResult<TResultData>>, ICoreRequestBase
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return DataResult<TResultData>.MakeFailure();
        }
    }

    public interface ICoreDataRequestHandler<TCommand, TResultData>
        : IRequestHandler<TCommand, DataResult<TResultData>>
        where TCommand : ICoreDataRequest<TResultData>
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchDataRequest<TBatchError, TResultData>
        : IRequest<BatchDataResult<TBatchError, TResultData>>, ICoreRequestBase
        where TBatchError : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchDataResult<TBatchError, TResultData>.MakeFailure();
        }
    }

    public interface ICoreBatchDataRequestHandler<TCommand, TBatchError, TResultData>
        : IRequestHandler<TCommand, BatchDataResult<TBatchError, TResultData>>
        where TCommand : ICoreBatchDataRequest<TBatchError, TResultData>
        where TBatchError : struct
        where TResultData : class
    { }

    /// <summary>
    /// request를 사용할 수 있는 사용자를 정의하는 attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
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
