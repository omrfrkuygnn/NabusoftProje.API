using Microsoft.AspNetCore.Mvc;
using NabusoftProje.API.DTOs;
using NabusoftProje.API.Models;
using NabusoftProje.API.Services;
using Microsoft.EntityFrameworkCore;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _db;
        private readonly PasswordHasher _passwordHasher;
        private readonly EmailService _emailService;

        public AuthController(JwtService jwtService, AppDbContext db, PasswordHasher passwordHasher, EmailService emailService)
        {
            _jwtService = jwtService;
            _db = db;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        // POST: /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, dto.Password))
                return Unauthorized("Geçersiz e-posta veya şifre.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        // POST: /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Bu e-posta ile kayıtlı bir kullanıcı zaten var.");
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Bu kullanıcı adı zaten alınmış.");

            var user = new User
            {
                FullName = dto.FullName,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                PhotoPath = dto.PhotoPath,
                BirthDate = dto.BirthDate
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }

        // POST: /api/auth/verify-email
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerificationDto dto)
        {
            var verification = await _db.EmailVerifications.FirstOrDefaultAsync(e => e.Email == dto.Email && e.Token == dto.Token && !e.IsUsed && e.Expiration > DateTime.UtcNow);
            if (verification == null)
                return BadRequest("Geçersiz veya süresi dolmuş doğrulama kodu.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");
            user.IsEmailVerified = true;
            verification.IsUsed = true;
            await _db.SaveChangesAsync();
            return Ok("E-posta doğrulandı.");
        }

        // POST: /api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Ok(); // Güvenlik için her zaman aynı yanıt
            var token = Guid.NewGuid().ToString("N");
            var reset = new PasswordResetToken
            {
                UserId = user.Id,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };
            _db.PasswordResetTokens.Add(reset);
            await _db.SaveChangesAsync();
            await _emailService.SendEmailAsync(user.Email, "Şifre Sıfırlama", $"Şifre sıfırlama kodunuz: {token}");
            return Ok();
        }

        // POST: /api/auth/resend-verification
        [HttpPost("resend-verification")]
        public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Ok();
            if (user.IsEmailVerified)
                return BadRequest("E-posta zaten doğrulanmış.");
            var token = Guid.NewGuid().ToString("N");
            var verification = new EmailVerification
            {
                UserId = user.Id,
                Email = user.Email,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                IsUsed = false
            };
            _db.EmailVerifications.Add(verification);
            await _db.SaveChangesAsync();
            await _emailService.SendEmailAsync(user.Email, "E-posta Doğrulama", $"Doğrulama kodunuz: {token}");
            return Ok();
        }

        // POST: /api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var reset = await _db.PasswordResetTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == dto.Token && !r.IsUsed && r.Expiration > DateTime.UtcNow);
            if (reset == null || reset.User.Email != dto.Email)
                return BadRequest("Geçersiz veya süresi dolmuş kod.");
            reset.User.PasswordHash = _passwordHasher.HashPassword(dto.NewPassword);
            reset.IsUsed = true;
            await _db.SaveChangesAsync();
            return Ok("Şifre güncellendi.");
        }
    }
} 