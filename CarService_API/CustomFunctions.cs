using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CarService_API
{
    public static class CustomFunctions
    {
        public static bool VerifyJwtSigningKey(string tokenString)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var securtityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSettings.Key));
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securtityKey,
                    ValidAudience = TokenSettings.Audience,
                    ValidIssuer = TokenSettings.Issuer
                };

                jwtTokenHandler.ValidateToken(tokenString, tokenValidationParameters, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static AppModel? ReadToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception();
                }
                if (!VerifyJwtSigningKey(token))
                {
                    throw new Exception();
                }
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                if (jwt.ValidTo < DateTime.UtcNow)
                {
                    throw new Exception();
                }
                return new AppModel
                {
                    UserId = Convert.ToDecimal(jwt.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0"),
                    CompanyId = Convert.ToDecimal(jwt.Claims.FirstOrDefault(x => x.Type == "CompanyId")?.Value ?? "0"),
                    UserType = jwt.Claims.FirstOrDefault(x => x.Type == "CompanyId")?.Value ?? "0"
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class ResultModel<T> : ResultModel //where T : class
    {
        public T Data { get; set; }
    }
    public class ResultModel
    {
        public string Message { get; set; } = "";
        public bool Status { get; set; } = false;
        public string StringValue1 { get; set; } = "";
        public long LongValue1 { get; set; } = 0;
    }
    public class TokenSettings
    {
        public static string Key { get; set; }
        public static string Audience { get; set; }
        public static string Issuer { get; set; }
    }
}
