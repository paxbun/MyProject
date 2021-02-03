using MyProject.Models;

namespace MyProject.Core.ViewModels
{
    public record UserIdentity
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// 유저 본명
        /// </summary>
        public string Username { get; init; }

        /// <summary>
        /// 유저 본명
        /// </summary>
        public string RealName { get; init; }

        /// <summary>
        /// 사용자 종류
        /// </summary>
        public UserType Type { get; set; }

        public override string ToString() => $"{Type}({Id}, {Username}, {RealName})";

        public static UserIdentity FromUser(User user) =>
            new UserIdentity
            {
                Id = user.Id,
                Username = user.Username,
                RealName = user.RealName,
                Type = user.Type,
            };
    }
}
