using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NabusoftProje.API.Models;
using NabusoftProje.API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;
        public AdminController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _db.Users.ToListAsync();
            return Ok(users);
        }

        // POST: /api/admin/roles
        [HttpPost("roles")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null) return NotFound();
            user.Role = dto.Role;
            await _db.SaveChangesAsync();
            return Ok("Rol atandı.");
        }

        // DELETE: /api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return Ok("Kullanıcı silindi.");
        }
    }
} 