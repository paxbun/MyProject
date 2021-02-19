using MyProject.Models;
using System.Linq;

namespace MyProject.Infrastructure.Services
{
    public static class CoreDbContextSeeder
    {
        /// <summary>
        /// 데이터베이스에 초기 데이터를 추가합니다.
        /// </summary>
        /// <param name="context">데이터베이스</param>
        public static void PopulateSeedData(
            this CoreDbContext context,
            User[] users)
        {
            foreach (var user in users)
            {
                if (user.Username is not null
                    && context.Set<User>().FirstOrDefault(u => u.Username == user.Username) is null)
                    context.Set<User>().Add(user);
            }
            context.SaveChanges();
        }
    }
}
