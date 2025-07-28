using EtkinlikKatilimApi.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EtkinlikKatilimApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritaban� ba�lant�s�
builder.Services.AddDbContext<EventDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. JWT Authentication ayarlar� (tek ve net �ekilde)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // appsettings.json'dan al
        };
    });

// CORS ayarları (geliştirme için tüm originlere izin ver)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Yetkilendirme servisi
builder.Services.AddAuthorization();

// 4. Controller ve Swagger/OpenAPI servisi
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();

var app = builder.Build();

// 5. Development ortam�nda Swagger aktif et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 6. HTTPS y�nlendirme, kimlik do�rulama ve yetkilendirme middleware'leri
app.UseHttpsRedirection();

app.UseCors(); // CORS'u authentication'dan önce ekle
app.UseAuthentication(); // JWT middleware zorunlu
app.UseAuthorization();

app.MapControllers();

app.Run();
