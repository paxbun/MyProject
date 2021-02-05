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
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreRequest<TReason, TResultData>
        : IRequest<Result<TReason, TResultData>>, ICoreRequestBase
        where TReason : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return Result<TReason, TResultData>.MakeFailure();
        }
    }

    public interface ICoreRequestHandler<TCommand, TReason, TResultData>
        : IRequestHandler<TCommand, Result<TReason, TResultData>>
        where TCommand : ICoreRequest<TReason, TResultData>
        where TReason : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchRequest<TBatchReason, TReason, TResultData>
        : IRequest<BatchResult<TBatchReason, TReason, TResultData>>, ICoreRequestBase
        where TBatchReason : struct
        where TReason : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchResult<TBatchReason, TReason, TResultData>.MakeFailure();
        }
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchReason, TReason, TResultData>
        : IRequestHandler<TCommand, BatchResult<TBatchReason, TReason, TResultData>>
        where TCommand : ICoreBatchRequest<TBatchReason, TReason, TResultData>
        where TBatchReason : struct
        where TReason : struct
        where TResultData : class
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    public interface ICoreRequest<TReason>
        : IRequest<Result<TReason>>, ICoreRequestBase
        where TReason : struct
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return Result<TReason>.MakeFailure();
        }
    }

    public interface ICoreRequestHandler<TCommand, TReason>
        : IRequestHandler<TCommand, Result<TReason>>
        where TCommand : ICoreRequest<TReason>
        where TReason : struct
    { }

    /// <summary>
    /// MyProject.Core에 들어있는 묶음 연산 request가 구현하는 인터페이스
    /// </summary>
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchRequest<TBatchReason, TReason>
        : IRequest<BatchResult<TBatchReason, TReason>>, ICoreRequestBase
        where TBatchReason : struct
        where TReason : struct
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchResult<TBatchReason, TReason>.MakeFailure();
        }
    }

    public interface ICoreBatchRequestHandler<TCommand, TBatchReason, TReason>
        : IRequestHandler<TCommand, BatchResult<TBatchReason, TReason>>
        where TCommand : ICoreBatchRequest<TBatchReason, TReason>
        where TBatchReason : struct
        where TReason : struct
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
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public interface ICoreBatchDataRequest<TBatchReason, TResultData>
        : IRequest<BatchDataResult<TBatchReason, TResultData>>, ICoreRequestBase
        where TBatchReason : struct
        where TResultData : class
    {
        object ICoreRequestBase.MakeDefaultFailure()
        {
            return BatchDataResult<TBatchReason, TResultData>.MakeFailure();
        }
    }

    public interface ICoreBatchDataRequestHandler<TCommand, TBatchReason, TResultData>
        : IRequestHandler<TCommand, BatchDataResult<TBatchReason, TResultData>>
        where TCommand : ICoreBatchDataRequest<TBatchReason, TResultData>
        where TBatchReason : struct
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
