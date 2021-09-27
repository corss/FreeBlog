using FreeBlog.Common;
using FreeBlog.Model.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace FreeBlog.Api.Extensions
{
    public class JwtHelper
    {
        public static readonly string Issuer = "Pi";          // 发行人
        public static readonly string Audience = "user";      // 订阅人
        public static readonly string Secret = "8TmJ2g24*gp5tdcqibZJgRlAQ7T93mOS666";    // 密钥

        /// <summary>
        /// 颁发JWT字符串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        public static TokenInfoViewModel IssueJwt(TokenModelJwt tokenModel)
        {

            //var claims = new Claim[] //old
            var claims = new List<Claim>
             {
              /*
              * 特别重要：
                1、这里将用户的部分信息，比如 uid 存到了Claim 中，如果你想知道如何在其他地方将这个 uid从 Token 中取出来，请看下边的SerializeJwt() 方法，或者在整个解决方案，搜索这个方法，看哪里使用了！
                2、你也可以研究下 HttpContext.User.Claims ，具体的你可以看看 Policys/PermissionHandler.cs 类中是如何使用的。
              */
             new Claim(JwtRegisteredClaimNames.Jti, tokenModel.Uid.ToString()),
             new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
             new Claim(JwtRegisteredClaimNames.Nbf,$"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
             //这个就是过期时间，目前是过期1000秒，可自定义，注意JWT有自己的缓冲过期时间
             new Claim (JwtRegisteredClaimNames.Exp,$"{new DateTimeOffset(DateTime.Now.AddSeconds(1200)).ToUnixTimeSeconds()}"),
             new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(1000).ToString()),
             new Claim(JwtRegisteredClaimNames.Iss,Issuer),
             new Claim(JwtRegisteredClaimNames.Aud,Audience),
         
             //new Claim(ClaimTypes.Role,tokenModel.Role),//为了解决一个用户多个角色(比如：Admin,System)，用下边的方法
            };

            // 可以将一个用户的多个角色全部赋予；
            // 作者：DX 提供技术支持；
            claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));


            //秘钥 (SymmetricSecurityKey 对安全性的要求，密钥的长度太短会报出异常)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var timec = new DateTimeOffset(DateTime.Now.AddMinutes(20)).ToUnixTimeMilliseconds();
            var jwt = new JwtSecurityToken(
                issuer: Issuer,
                claims: claims,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);
            //打包返回前台
            var responseJson = new TokenInfoViewModel
            {
                success = true,
                token = encodedJwt,
                expireTime = C.String(timec),
                token_type = "Bearer"
            };
            return responseJson;
        }


        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static TokenModelJwt SerializeJwt(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            var tm = new TokenModelJwt
            {
                Uid = C.Int(jwtToken.Id),
                Role = role != null ? role + "" : "",
            };
            return tm;
        }
        /// <summary>
        /// 解析 获取ID
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public static int SerializeJwt2(string jwtStr)
        {
            var jwtHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
            object role;
            try
            {
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return C.Int(jwtToken.Id);
        }

        /// <summary>
        /// 令牌
        /// </summary>
        public class TokenModelJwt
        {
            /// <summary>
            /// Id
            /// </summary>
            public long Uid { get; set; }
            /// <summary>
            /// 角色
            /// </summary>
            public string Role { get; set; }
            /// <summary>
            /// 职能
            /// </summary>
            public string Work { get; set; }
        }
    }
}
