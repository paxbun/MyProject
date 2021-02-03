using MyProject.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Services
{
    public interface ICoreDbContext
    {
        /// <summary>
        /// 사용자 목록
        /// </summary>
        DbSet<User> Users { get; set; }

        /// <summary>
        /// 변경사항을 저장합니다. DB를 변경한 후 반드시 호출하여야 합니다.
        /// </summary>
        /// <param name="cancellationToken">테스크의 진행 상황을 관찰하기 위한 인스턴스</param>
        /// <returns>변경시 영향이 간 행 수</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 변경사항을 저장합니다. DB를 변경한 후 반드시 호출하여야 합니다.
        /// </summary>
        /// <returns>변경시 영향이 간 행 수</returns>
        int SaveChanges();
    }
}
