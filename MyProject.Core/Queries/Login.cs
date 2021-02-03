using MyProject.Core.Services;
using MyProject.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
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
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Username == request.Username, cancellationToken);

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
