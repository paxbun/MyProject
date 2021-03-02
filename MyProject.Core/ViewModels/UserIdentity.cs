using MyProject.Models;
using System.Net;

namespace MyProject.Core.ViewModels
{
    public record UserIdentity
    {
        /// <summary>
        /// 사용자 식별자
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 사용자가 입력하는 ID
        /// </summary>
        public string Username { get; init; }

        /// <summary>
        /// 사용자 본명
        /// </summary>
        public string RealName { get; init; }

        /// <summary>
        /// 사용자 종류
        /// </summary>
        public UserType Type { get; init; }

        /// <summary>
        /// 사용자 접속 IP
        /// </summary>
        public IPAddress Ip { get; init; }

        public static UserIdentity FromUser(User user, IPAddress ip) =>
            new UserIdentity
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Type = user.Type,
                Ip = ip
            };
    }
}
