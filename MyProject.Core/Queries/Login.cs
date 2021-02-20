using Microsoft.EntityFrameworkCore;
using MyProject.Core.Services;
using MyProject.Core.ViewModels;
using MyProject.Models;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace MyProject.Core.Queries
{
    [For(AllowAnonymous = true)]
    public class LoginQuery : CoreDataRequest<LoginResultView>
    {
        /// <summary>
        /// 사용자 ID
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 사용자 비밀번호 평문
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 접속 IP
        /// </summary>
        public IPAddress Ip { get; set; }
    }

    public class LoginQueryFormatter : ICoreLoggerFormatter<LoginQuery>
    {
        public object Format(LoginQuery obj) => new
        {
            obj.Username
        };
    }

    public class LoginQueryHandler : ICoreDataRequestHandler<LoginQuery, LoginResultView>
    {
        private readonly ICoreDbContext _dbContext;
        private readonly IUserIdentityService _identityService;
        private readonly ICoreLogger _logger;
        public LoginQueryHandler(ICoreDbContext dbContext, IUserIdentityService identityService, ICoreLogger logger)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<DataResult<LoginResultView>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (request.Ip is null)
                return request.MakeFailure();

            var user = await _dbContext.Set<User>().FirstOrDefaultAsync(user => user.Username == request.Username, cancellationToken);

            if (user is null)
                return request.MakeFailure();

            if (!user.VerifyPassword(request.Password))
                return request.MakeFailure();

            var identity = UserIdentity.FromUser(user, request.Ip);

            await _logger.LogInPlaceOfResponseAsync(
                request, DataResult<UserIdentity>.MakeSuccess(identity), cancellationToken);

            return request.MakeSuccess(new LoginResultView
            {
                AccessToken = _identityService.GenerateToken(identity, TokenType.AccessToken),
                RefreshToken = _identityService.GenerateToken(identity, TokenType.RefreshToken),
            });
        }
    }

}
