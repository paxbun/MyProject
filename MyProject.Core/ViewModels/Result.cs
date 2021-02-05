#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    /// 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchReason">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TReason">개별 연산 자체가 실패한 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record BatchResult<TBatchReason, TReason, TResultData>
        where TBatchReason : struct
        where TReason : struct
        where TResultData : class
    {
        public static BatchResult<TBatchReason, TReason, TResultData> MakeSuccess(
            IEnumerable<Result<TReason, TResultData>>? enumerable = default)
        {
            return new BatchResult<TBatchReason, TReason, TResultData>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<Result<TReason, TResultData>>()
            };
        }

        public static BatchResult<TBatchReason, TReason, TResultData> MakeFailure(
            TBatchReason reason = default)
        {
            return new BatchResult<TBatchReason, TReason, TResultData>
            {
                Success = false,
                Reason = reason,
                _results = Array.Empty<Result<TReason, TResultData>>()
            };
        }

        public BatchResult<TBatchReason, TReason, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new BatchResult<TBatchReason, TReason, TAnotherResultData>
                {
                    Success = true,
                    _results = _results.Select(result => result.Select(func)).ToArray()
                };
            }
            else
            {
                return new BatchResult<TBatchReason, TReason, TAnotherResultData>
                {
                    Success = false,
                    Reason = Reason,
                    _results = Array.Empty<Result<TReason, TAnotherResultData>>()
                };
            }
        }

        public bool Success { get; init; }

        public TBatchReason? Reason { get; init; }

#pragma warning disable CS8618
        private Result<TReason, TResultData>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<Result<TReason, TResultData>> Results { get => new(_results); }
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
    /// 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchReason">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TReason">개별 연산 자체가 실패한 이유를 나타내는 열거형</typeparam>
    public record BatchResult<TBatchReason, TReason>
        where TBatchReason : struct
        where TReason : struct
    {
        public static BatchResult<TBatchReason, TReason> MakeSuccess(
            IEnumerable<Result<TReason>>? enumerable = default)
        {
            return new BatchResult<TBatchReason, TReason>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<Result<TReason>>()
            };
        }

        public static BatchResult<TBatchReason, TReason> MakeFailure(
            TBatchReason reason = default)
        {
            return new BatchResult<TBatchReason, TReason>
            {
                Success = false,
                Reason = reason,
                _results = Array.Empty<Result<TReason>>()
            };
        }

        public bool Success { get; init; }

        public TBatchReason? Reason { get; init; }

#pragma warning disable CS8618
        private Result<TReason>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<Result<TReason>> Results { get => new(_results); }
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

    /// <summary>
    /// 개별 연산이 실패시 이유를 나타내는 경우가 없는 묶음 연산 결과를 나타내는 클래스
    /// </summary>
    /// <typeparam name="TBatchReason">묶음 연산 자체가 실패한 경우 이유를 나타내는 열거형</typeparam>
    /// <typeparam name="TResultData">성공시 추가 데이터</typeparam>
    public record BatchDataResult<TBatchReason, TResultData>
        where TBatchReason : struct
        where TResultData : class
    {
        public static BatchDataResult<TBatchReason, TResultData> MakeSuccess(
            IEnumerable<DataResult<TResultData>>? enumerable = default)
        {
            return new BatchDataResult<TBatchReason, TResultData>
            {
                Success = true,
                _results = enumerable?.ToArray() ?? Array.Empty<DataResult<TResultData>>()
            };
        }

        public static BatchDataResult<TBatchReason, TResultData> MakeFailure(
            TBatchReason reason = default)
        {
            return new BatchDataResult<TBatchReason, TResultData>
            {
                Success = false,
                Reason = reason,
                _results = Array.Empty<DataResult<TResultData>>()
            };
        }

        public BatchDataResult<TBatchReason, TAnotherResultData> Select<TAnotherResultData>(
            Func<TResultData?, TAnotherResultData?>? func = default)
            where TAnotherResultData : class
        {
            if (Success)
            {
                return new BatchDataResult<TBatchReason, TAnotherResultData>
                {
                    Success = true,
                    _results = _results.Select(DataResult => DataResult.Select(func)).ToArray()
                };
            }
            else
            {
                return new BatchDataResult<TBatchReason, TAnotherResultData>
                {
                    Success = false,
                    Reason = Reason,
                    _results = Array.Empty<DataResult<TAnotherResultData>>()
                };
            }
        }

        public bool Success { get; init; }

        public TBatchReason? Reason { get; init; }

#pragma warning disable CS8618
        private DataResult<TResultData>[] _results;
#pragma warning restore CS8618
        public ReadOnlyCollection<DataResult<TResultData>> Results { get => new(_results); }
    }

}
