using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyProject.Models
{
    public enum UserType
    {
        /// <summary>
        /// 일반 사용자
        /// </summary>
        General = 1,

        /// <summary>
        /// 시스템 관리자
        /// </summary>
        Administrator = 5,
    }

    /// <summary>
    /// 사용자 정보
    /// </summary>
    public abstract class User
    {
        /// <summary>
        /// 사용자 식별자
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 사용자가 입력하는 ID
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 암호화된 비밀번호
        /// </summary>
        public string EncryptedPassword { get; set; }

        /// <summary>
        /// 본명
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 사용자 종류
        /// </summary>
        public UserType Type { get; set; }

        /// <summary>
        /// 내부적으로 <c>SetPassword</c>를 활용
        /// </summary>
        public string Password { set => SetPassword(value); }

        /// <summary>
        /// 비밀번호를 설정합니다.
        /// </summary>
        /// <param name="plainPassword">비밀번호 평문</param>
        public void SetPassword(string plainPassword)
        {
            EncryptedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        /// <summary>
        /// 비밀번호가 맞는지 확인합니다.
        /// </summary>
        /// <param name="plainPassword">비밀번호 평문</param>
        public bool VerifyPassword(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, EncryptedPassword);
        }
    }

    /// <summary>
    /// 일반 사용자 정보
    /// </summary>
    public class GeneralUser : User { }

    /// <summary>
    /// 관리자 사용자 정보
    /// </summary>
    public class AdministratorUser : User { }

    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.Property(user => user.Username)
                .IsRequired(true)
                .HasMaxLength(128)
                .IsUnicode(false);
            builder.HasAlternateKey(user => user.Username);
            builder.Property(user => user.EncryptedPassword)
                .IsRequired(true)
                .HasMaxLength(128)
                .IsUnicode(false);
            builder.Property(user => user.RealName)
                .IsRequired(true)
                .HasMaxLength(128)
                .IsUnicode(false);
            builder.HasDiscriminator(user => user.Type)
                .HasValue<GeneralUser>(UserType.General)
                .HasValue<AdministratorUser>(UserType.Administrator);
        }
    }

    public class GeneralUserEntityTypeConfiguration : IEntityTypeConfiguration<GeneralUser>
    {
        public void Configure(EntityTypeBuilder<GeneralUser> builder) { }
    }


    public class AdministratorUserEntityTypeConfiguration
        : IEntityTypeConfiguration<AdministratorUser>
    {
        public void Configure(EntityTypeBuilder<AdministratorUser> builder) { }
    }

}
