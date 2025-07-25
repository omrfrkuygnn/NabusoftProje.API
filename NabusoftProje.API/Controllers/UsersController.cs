using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NabusoftProje.API.Models;
using NabusoftProje.API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UsersController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/users/me
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userId || u.Id.ToString() == userId);
            if (user == null) return NotFound();
            return Ok(new {
                user.Id,
                user.FullName,
                user.Username,
                user.Email,
                user.BirthDate,
                user.PhotoPath,
                user.Role,
                user.IsEmailVerified
            });
        }

        // PUT: /api/users/me
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userId || u.Id.ToString() == userId);
            if (user == null) return NotFound();
            user.FullName = dto.FullName ?? user.FullName;
            user.PhotoPath = dto.PhotoPath ?? user.PhotoPath;
            user.BirthDate = dto.BirthDate ?? user.BirthDate;
            await _db.SaveChangesAsync();
            return Ok("Profil güncellendi.");
        }

        // GET: /api/users/roles
        [Authorize(Roles = "Admin")]
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = new[] { "Admin", "User" };
            return Ok(roles);
        }
    }
} 