#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// 연산 결과를 나타내는 객체가 구현하는 인터페이스
    /// </summary>
    public interface IResultBase
    {
        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool Success { get; init; }
    }

    /// <summary>
    /// 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record Result<TError, TResultData> : IResultBase
        where TError : struct
        where TResultData : class
    {
        public static Result<TError, TResultData> MakeSuccess(TResultData? data = default)
        {
            return new Result<TError, TResultData>
            {
                Success = true,
                Error = null,
                Data = data
            };
        }

        public static Result<TError, TResultData> MakeFailure(TError Error = default)
        {
            return new Result<TError, TResultData>
            {
                Success = false,
                Error = Error
            };
        }

        public Result<TError, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new Result<TError, TAnotherResultData>
                {
                    Success = true,
                    Error = null,
                    Data = func?.Invoke(Data)
                };
            }
            else
            {
                return new Result<TError, TAnotherResultData>
                {
                    Success = false,
                    Error = Error
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
        public TError? Error { get; init; }

        /// <summary>
        /// 결과 데이터
        /// </summary>
        public TResultData? Data { get; init; }
    }

    /// <summary>
    /// 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TError">개별 연산 자체가 실패한 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record BatchResult<TBatchError, TError, TResultData> : IResultBase
        where TBatchError : struct
        where TError : struct
        where TResultData : class
    {
        public static BatchResult<TBatchError, TError, TResultData> MakeSuccess(
            IEnumerable<Result<TError, TResultData>>? enumerable = default)
        {
            return new BatchResult<TBatchError, TError, TResultData>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<Result<TError, TResultData>>()
            };
        }

        public static BatchResult<TBatchError, TError, TResultData> MakeFailure(
            TBatchError error = default)
        {
            return new BatchResult<TBatchError, TError, TResultData>
            {
                Success = false,
                Error = error,
                _results = Array.Empty<Result<TError, TResultData>>()
            };
        }

        public BatchResult<TBatchError, TError, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new BatchResult<TBatchError, TError, TAnotherResultData>
                {
                    Success = true,
                    _results = _results.Select(result => result.Select(func)).ToArray()
                };
            }
            else
            {
                return new BatchResult<TBatchError, TError, TAnotherResultData>
                {
                    Success = false,
                    Error = Error,
                    _results = Array.Empty<Result<TError, TAnotherResultData>>()
                };
            }
        }

        public bool Success { get; init; }

        public TBatchError? Error { get; init; }

#pragma warning disable CS8618
        private Result<TError, TResultData>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<Result<TError, TResultData>> Results { get => new(_results); }
    }

    /// <summary>
    /// 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TError">실패시 이유를 나타내는 열거형</typeparam>
    public record Result<TError> : IResultBase
        where TError : struct
    {
        public static Result<TError> MakeSuccess()
        {
            return new Result<TError>
            {
                Success = true,
                Error = null,
            };
        }

        public static Result<TError> MakeFailure(TError Error = default)
        {
            return new Result<TError>
            {
                Success = false,
                Error = Error
            };
        }

        /// <summary>
        /// 성공 여부
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// 실패한 이유
        /// </summary>
        public TError? Error { get; init; }
    }

    /// <summary>
    /// 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TError">개별 연산 자체가 실패한 이유를 나타내는 열거형</typeparam>
    public record BatchResult<TBatchError, TError> : IResultBase
        where TBatchError : struct
        where TError : struct
    {
        public static BatchResult<TBatchError, TError> MakeSuccess(
            IEnumerable<Result<TError>>? enumerable = default)
        {
            return new BatchResult<TBatchError, TError>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<Result<TError>>()
            };
        }

        public static BatchResult<TBatchError, TError> MakeFailure(
            TBatchError error = default)
        {
            return new BatchResult<TBatchError, TError>
            {
                Success = false,
                Error = error,
                _results = Array.Empty<Result<TError>>()
            };
        }

        public bool Success { get; init; }

        public TBatchError? Error { get; init; }

#pragma warning disable CS8618
        private Result<TError>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<Result<TError>> Results { get => new(_results); }
    }

    /// <summary>
    /// 실패시 이유를 나타내는 경우가 없는 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record DataResult<TResultData> : IResultBase
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

    /// <summary>
    /// 개별 연산이 실패시 이유를 나타내는 경우가 없는 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchError">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record BatchDataResult<TBatchError, TResultData> : IResultBase
        where TBatchError : struct
        where TResultData : class
    {
        public static BatchDataResult<TBatchError, TResultData> MakeSuccess(
            IEnumerable<DataResult<TResultData>>? enumerable = default)
        {
            return new BatchDataResult<TBatchError, TResultData>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<DataResult<TResultData>>()
            };
        }

        public static BatchDataResult<TBatchError, TResultData> MakeFailure(
            TBatchError error = default)
        {
            return new BatchDataResult<TBatchError, TResultData>
            {
                Success = false,
                Error = error,
                _results = Array.Empty<DataResult<TResultData>>()
            };
        }

        public BatchDataResult<TBatchError, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new BatchDataResult<TBatchError, TAnotherResultData>
                {
                    Success = true,
                    _results = _results.Select(DataResult => DataResult.Select(func)).ToArray()
                };
            }
            else
            {
                return new BatchDataResult<TBatchError, TAnotherResultData>
                {
                    Success = false,
                    Error = Error,
                    _results = Array.Empty<DataResult<TAnotherResultData>>()
                };
            }
        }

        public bool Success { get; init; }

        public TBatchError? Error { get; init; }

#pragma warning disable CS8618
        private DataResult<TResultData>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<DataResult<TResultData>> Results { get => new(_results); }
    }

}
