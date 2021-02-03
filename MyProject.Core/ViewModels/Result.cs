#nullable enable

using System;

namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record Result<TReason, TResultData>
        where TReason : struct
        where TResultData : class
    {
        public static Result<TReason, TResultData> MakeSuccess(TResultData? data = default)
        {
            return new Result<TReason, TResultData>
            {
                Success = true,
                Reason = null,
                Data = data
            };
        }

        public static Result<TReason, TResultData> MakeFailure(TReason reason = default)
        {
            return new Result<TReason, TResultData>
            {
                Success = false,
                Reason = reason
            };
        }

        public Result<TReason, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new Result<TReason, TAnotherResultData>
                {
                    Success = true,
                    Reason = null,
                    Data = func?.Invoke(Data)
                };
            }
            else
            {
                return new Result<TReason, TAnotherResultData>
                {
                    Success = false,
                    Reason = Reason
                };
            }
        }

        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// 실패한 이유
        /// </summary>
        public TReason? Reason { get; init; }

        /// <summary>
        /// 결과 데이터
        /// </summary>
        public TResultData? Data { get; init; }
    }

    /// <summary>
    /// 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TReason">실패시 이유를 나타내는 열거형</typeparam>
    public record Result<TReason>
        where TReason : struct
    {
        public static Result<TReason> MakeSuccess()
        {
            return new Result<TReason>
            {
                Success = true,
                Reason = null,
            };
        }

        public static Result<TReason> MakeFailure(TReason reason = default)
        {
            return new Result<TReason>
            {
                Success = false,
                Reason = reason
            };
        }

        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// 실패한 이유
        /// </summary>
        public TReason? Reason { get; init; }
    }

    /// <summary>
    /// 실패시 이유를 나타내는 경우가 없는 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record DataResult<TResultData>
        where TResultData : class
    {
        public static DataResult<TResultData> MakeSuccess(TResultData? data = default)
        {
            return new DataResult<TResultData>
            {
                Success = true,
                Data = data
            };
        }

        public static DataResult<TResultData> MakeFailure()
        {
            return new DataResult<TResultData>
            {
                Success = false
            };
        }

        public DataResult<TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new DataResult<TAnotherResultData>
                {
                    Success = true,
                    Data = func?.Invoke(Data)
                };
            }
            else
            {
                return new DataResult<TAnotherResultData>
                {
                    Success = false,
                };
            }
        }

        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// 결과 데이터
        /// </summary>
        public TResultData? Data { get; init; }
    }
}
