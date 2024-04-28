using CarService_API;
using CarService_API.Models.DB;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddDbContext<ModelContext>();
builder.Services.AddTransient<Extentsion>();

TokenSettings.Issuer = builder.Configuration["Jwt:Issuer"];
TokenSettings.Audience = builder.Configuration["Jwt:Audience"];
TokenSettings.Key = builder.Configuration["Jwt:Key"];

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseCors("corsapp");

app.UseMiddleware<MyMiddleware>();

app.MapControllers();

app.Run();