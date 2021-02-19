﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// Pagination 구현을 위한 helper 클래스
    /// </summary>
    public record Page<T>
    {
        private static IQueryable<T> MakeWindowedQueryable(IQueryable<T> queryable, int? beginIdx, int? endIdx)
        {
            if (beginIdx is not null)
                queryable = queryable.Skip((int)beginIdx);

            if (endIdx is not null)
                queryable = queryable.Take((int)endIdx - beginIdx ?? 0);

            return queryable;
        }

        public static async Task<Page<T>> MakeCountOnlyResultAsync(
            IQueryable<T> queryable, int? beginIdx, int? endIdx, CancellationToken cancellationToken = default)
        {
            return new Page<T>
            {
                Count = await MakeWindowedQueryable(queryable, beginIdx, endIdx).CountAsync(cancellationToken)
            };
        }

        public static async Task<Page<T>> MakeResultAsync(
            IQueryable<T> queryable, int? beginIdx, int? endIdx, CancellationToken cancellationToken = default)
        {
            var results = await MakeWindowedQueryable(queryable, beginIdx, endIdx).ToListAsync(cancellationToken);
            return new Page<T>
            {
                Results = results,
                Count = results.Count,
                BeginIdx = beginIdx,
                EndIdx = endIdx
            };
        }

        /// <summary>
        /// 결과 목록
        /// </summary>
        public IList<T> Results { get; init; }

        /// <summary>
        /// 요소 개수
        /// </summary>
        public int Count { get; init; }

        /// <summary>
        /// 시작 인덱스
        /// </summary>
        public int? BeginIdx { get; init; }

        /// <summary>
        /// 끝 인덱스
        /// </summary>
        public int? EndIdx { get; init; }
    }
}