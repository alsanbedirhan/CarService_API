using CarService_API.Models.DB;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarService_API
{
    public class Extentsion
    {
        IHttpContextAccessor _http;
        ModelContext _context;
        IMemoryCache _cache;
        public Extentsion(IHttpContextAccessor http, ModelContext context, IMemoryCache cache)
        {
            _context = context;
            _http = http;
            _cache = cache;
        }

        public string GetToken()
        {
            try
            {
                string token = _http.HttpContext.Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception();
                }
                if (token.StartsWith("Bearer"))
                {
                    token = token.Remove(0, 7);
                }
                return token;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public AppModel? GetTokenValues()
        {
            return CustomFunctions.ReadToken(GetToken());
        }

        public string GetDeviceId()
        {
            try
            {
                return _http.HttpContext.Request.Headers["Device"].ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string GetUserAgent()
        {
            try
            {
                return _http.HttpContext.Request.Headers["User-Agent"].ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public string GetVersion()
        {
            try
            {
                return _http.HttpContext.Request.Headers["App-Version"].ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        //public async Task<int?> Auth()
        //{
        //    try
        //    {
        //        var a = GetTokenValues();
        //        if (a == null) { throw new Exception(); }
        //        var user = await _tcontext.Personels.AsNoTracking().Where(x => x.Sicilno == a.SicilNo && x.Act == 1).Select(x => new { x.Sicilno, x.Departman, x.Gorevid, x.Altbolum }).FirstOrDefaultAsync();
        //        if (user == null) { throw new Exception(); }
        //        if (!a.Prg.HasValue) { throw new Exception(); };

        //        return await fncAuth((int)a.Prg.Value, user.Sicilno, user.Departman, user.Gorevid, user.Altbolum, (a.Prg.Value.GetPrg()?.Level ?? 0));
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //private async Task<int?> fncAuth(int prgid, int sicilno, int departman, int gorevid, int altbolum, int subid)
        //{
        //    try
        //    {
        //        if (!(await _tcontext.Personels.AnyAsync(x => x.Sicilno == sicilno && x.Act == 1)))
        //        {
        //            return null;
        //        }
        //        var p = await _tcontext.Prgaccesses.AsNoTracking().Where(x => x.Act == 1 && x.PrgId == prgid && x.SubId == subid &&
        //      ((x.Level & 1) > 0 ? x.Sicilno == sicilno : true) && ((x.Level & 2) > 0 ? x.Gorev == gorevid : true) &&
        //      ((x.Level & 4) > 0 ? x.Bolum == departman : true) && ((x.Level & 8) > 0 ? x.Altbolum == altbolum : true))
        //          .Select(x => new { x.Yetki, x.Level, x.Gorev, x.Act, x.PrgId, x.Bolum, x.Sicilno, x.Sirano }).OrderBy(x => x.Sirano).FirstOrDefaultAsync();
        //        if (p == null)
        //        {
        //            return null;
        //        }
        //        return Convert.ToInt32(p.Yetki);
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}
        //public string CreateTokenWithModel(AppModel? model)
        //{
        //    try
        //    {
        //        if (model == null) { throw new Exception(); }
        //        return CreateToken(model);
        //    }
        //    catch (Exception)
        //    {
        //        return "";
        //    }
        //}
        public string CreateToken(decimal UserId, decimal CompanyId, string UserType)
        {
            if (UserId <= 0)
            {
                return "";
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
             new Claim("UserId", UserId.ToString()),
             new Claim("CompanyId", CompanyId.ToString()),
             new Claim("UserType", UserType)
            };
            var token = new JwtSecurityToken(TokenSettings.Issuer, TokenSettings.Audience,
                claims, null, DateTime.UtcNow.AddDays(7), credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        //public string CreateTokenWithContext()
        //{
        //    try
        //    {
        //        var model = GetTokenValues();
        //        if (model == null) { throw new Exception(); }
        //        return CreateTokenWithModel(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        return "";
        //    }
        //}
    }
    public class AppModel
    {
        public decimal UserId { get; set; } = 0;
        public decimal CompanyId { get; set; } = 0;
        public string UserType { get; set; } = "";
    }
    public class LoginResultUser
    {
        public decimal UserId { get; set; }
        public string UserType { get; set; }
        public string CompanyName { get; set; }
        public string Mail { get; set; }
        public decimal CompanyId { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Token { get; set; }
    }
    public class RequestModel
    {
        public decimal UserId { get; set; }
        public string CihazId { get; set; }
        public string User_Agent { get; set; }
        public string IpAdress { get; set; }
        public string Path { get; set; }
        public string AppVersion { get; set; }
        public string Query { get; set; }
        public DateTime Time { get; set; }
    }
}
