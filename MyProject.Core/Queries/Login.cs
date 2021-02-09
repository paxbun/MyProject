using Microsoft.EntityFrameworkCore;
using MyProject.Core.Services;
using MyProject.Core.ViewModels;
using MyProject.Models;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Commands
{
    [For(AllowAnonymous = true)]
    public record LoginQuery : ICoreDataRequest<LoginResultView>
    {
        public UserIdentity Identity { get; set; }

        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 사용자 비밀번호 평문
        /// </summary>
        public string Password { get; set; }
    }

    public class LoginQueryFormatter : ICoreLoggerFormatter<LoginQuery>
    {
        public record LoginQueryForLogger
        {
            public string Username { get; init; }
            public int PasswordLength { get; init; }
        }

        public object Format(LoginQuery obj)
        {
            return new LoginQueryForLogger
            {
                Username = obj.Username,
                PasswordLength = obj.Password?.Length ?? 0
            };
        }
    }

    public class LoginResultViewFormatter : ICoreLoggerFormatter<DataResult<LoginResultView>>
    {
        private readonly IUserIdentityService _identityService;

        public LoginResultViewFormatter(IUserIdentityService identityService)
            => _identityService = identityService;

        public object Format(DataResult<LoginResultView> obj)
        {
            return obj.Select(view => _identityService.ReadUserIdentity(view.AccessToken, TokenType.AccessToken));
        }
    }

    public class LoginQueryHandler : ICoreDataRequestHandler<LoginQuery, LoginResultView>
    {
        private readonly ICoreDbContext _dbContext;
        private readonly IUserIdentityService _identityService;

        public LoginQueryHandler(ICoreDbContext dbContext, IUserIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<DataResult<LoginResultView>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(user => user.Username == request.Username, cancellationToken);

            if (user == null)
                return DataResult<LoginResultView>.MakeFailure();

            if (!user.VerifyPassword(request.Password))
                return DataResult<LoginResultView>.MakeFailure();

            var identity = UserIdentity.FromUser(user);

            return DataResult<LoginResultView>.MakeSuccess(new LoginResultView
            {
                AccessToken = _identityService.GenerateToken(identity, TokenType.AccessToken),
                RefreshToken = _identityService.GenerateToken(identity, TokenType.RefreshToken),
            });
        }
    }

}
