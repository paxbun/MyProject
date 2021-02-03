namespace MyProject.Core.ViewModels
{
    /// <summary>
    /// 로그인 결과를 나타내는 클래스
    /// </summary>
    public record LoginResultView
    {
        /// <summary>
        /// accessToken
        /// </summary>
        public string AccessToken { get; init; }

        /// <summary>
        /// refreshToken
        /// </summary>
        public string RefreshToken { get; init; }
    }
}
