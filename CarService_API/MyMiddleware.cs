using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System;
using CarService_API.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace CarService_API
{
    public class MyMiddleware
    {
        private readonly List<string> IgnorePaths = new List<string> { "/api/noauth/register", "/api/noauth/login", "/api/update/file", "/api/update/token" };
        private readonly RequestDelegate _nextMiddleWare;
        public MyMiddleware(RequestDelegate next)
        {
            _nextMiddleWare = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Path.HasValue && context.Request.Path.Value.StartsWith("/api"))
                {
                    AppModel? s = null;
                    var _extention = context.RequestServices.GetService<Extentsion>();
                    var _context = context.RequestServices.GetService<ModelContext>();
                    var _cache = context.RequestServices.GetService<IMemoryCache>();
                    if (_extention == null || _context == null || _cache == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        return;
                    }
                    if (!IgnorePaths.Contains(context.Request.Path.Value))
                    {
                        s = _extention.GetTokenValues();
                        var _users = _cache.Get<List<User>>("users");
                        if (_users == null)
                        {
                            _users = await _context.Users.AsNoTracking().Include(x => x.Company).ToListAsync();
                            _cache.Set("users", _users);
                        }
                        if (s == null || !_users.Any(x => x.Id == s.UserId))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }
                    }
                    string deviceid = _extention.GetDeviceId(), host = context.Connection.RemoteIpAddress?.ToString() ?? "",
                        user_agent = _extention.GetUserAgent(), vs = _extention.GetVersion();

                    await Task.Run(() =>
                    {
                        try
                        {
                            var log = _cache.Get<List<RequestModel>>("LOG") ?? new List<RequestModel>();
                            log.Add(new RequestModel
                            {
                                Path = context.Request.Path,
                                UserId = s?.UserId ?? 0,
                                Time = DateTime.Now.ToLocalTime(),
                                CihazId = deviceid,
                                AppVersion = vs,
                                IpAdress = host,
                                User_Agent = user_agent,
                                Query = context.Request.QueryString.Value ?? "",
                            });
                            _cache.Set("LOG", log, TimeSpan.FromHours(1));
                        }
                        catch (Exception)
                        {

                        }
                    });
                }
                try
                {
                    await _nextMiddleWare(context);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
