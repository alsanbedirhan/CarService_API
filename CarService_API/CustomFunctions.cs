using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

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
                    UserType = jwt.Claims.FirstOrDefault(x => x.Type == "UserType")?.Value ?? "0"
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password + passwordSalt)));
            }
        }
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(computedHash) == storedHash;
            }
        }
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            return Regex.IsMatch(email, pattern);
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
