using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blog2022_netcore.Common
{
    public class JwtHelper
    {
        public static string CreateJwt(JwtModel jwtModel)
        {
            var keysBuilder = new ConfigurationBuilder().AddJsonFile("keys.json").Build();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keysBuilder["Jwt"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            SecurityToken securityToken = new JwtSecurityToken(
                issuer: "issuer",
                audience: "audience",
                signingCredentials: creds,
                expires: DateTime.Now.AddYears(1), //一年后过期
                claims: new Claim[] {
                   new Claim(ClaimTypes.Role,jwtModel.RoleName)
                }
                );
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }
        public static JwtModel SerializeJwt(string jwtStr)
        {
            //不校验，直接解析token
            var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(jwtStr);
            var tokenJwt = JsonConvert.DeserializeObject<JwtModel>(jwtToken.Payload.SerializeToJson());
            return tokenJwt;
        }
    }


    public class JwtModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
    }
}
