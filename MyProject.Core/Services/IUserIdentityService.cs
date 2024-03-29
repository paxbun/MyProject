﻿using MyProject.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace MyProject.Core.Services
{
    /// <summary>
    /// Token 종류를 나타내는 열거형
    /// </summary>
    public enum TokenType
    {
        RefreshToken,
        AccessToken
    }

    /// <summary>
    /// <c>IUserIdentityService</c> 내에서 인증이 실패한 경우 던져지는 예외
    /// </summary>
    public class UserIdentityServiceException : Exception
    { }

    /// <summary>
    /// accessToken과 refreshToken을 생성하는 인터페이스
    /// </summary>
    public interface IUserIdentityService
    {
        /// <summary>
        /// 주어진 token에서 사용자 로그인 정보를 읽습니다. token에 들어있는 IP 정보와 <c>ip</c>가 다르면 오류를 낼 수 있습니다.
        /// <c>type</c>이 <c>RefreshToken</c>이면 <c>ip</c>를 무시합니다.
        /// </summary>
        /// <param name="token">토큰</param>
        /// <param name="type">토큰 종류</param>
        /// <param name="ip">접속 IP</param>
        /// <returns>사용자 정보 객체</returns>
        public UserIdentity ReadUserIdentity(string token, TokenType type, IPAddress ip);

        /// <summary>
        /// 주어진 사용자 정보에서 token을 생성합니다.
        /// </summary>
        /// <param name="identity">사용자 정보</param>
        /// <param name="type">토큰 종류</param>
        /// <returns>토큰</returns>
        public string GenerateToken(UserIdentity identity, TokenType type);

        /// <summary>
        /// 주어진 사용자 정보로부터 <c>Claim</c> 배열을 생성합니다.
        /// </summary>
        /// <param name="identity">사용자 정보 객체</param>
        /// <returns><c>Claim</c> 배열</returns>
        public Claim[] ToClaims(UserIdentity identity);

        /// <summary>
        /// Claim 배열에서 사용자 정봅 객체를 생성합니다.
        /// </summary>
        /// <param name="claims"><c>Claim</c> 배열</param>
        /// <returns>사용자 정보 객체</returns>
        public UserIdentity FromClaims(IEnumerable<Claim> claims);
    }

}
