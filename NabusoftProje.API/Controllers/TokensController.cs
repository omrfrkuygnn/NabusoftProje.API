using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NabusoftProje.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokensController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TokensController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/tokens
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetBalance()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == userId || u.Id.ToString() == userId);
            if (user == null) return NotFound();
            return Ok(new { user.Tokens });
        }

        // GET: /api/tokens/history
        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var user = await _db.Users.Include(u => u.TokenTransactions).FirstOrDefaultAsync(u => u.Username == userId || u.Id.ToString() == userId);
            if (user == null) return NotFound();
            return Ok(user.TokenTransactions);
        }

        // POST: /api/tokens/grant
        [Authorize(Roles = "Admin")]
        [HttpPost("grant")]
        public async Task<IActionResult> GrantTokens([FromBody] GrantTokensDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == dto.UserId);
            if (user == null) return NotFound();
            user.Tokens += dto.Amount;
            _db.TokenTransactions.Add(new TokenTransaction { /*UserId = user.Id,*/ /*Amount = dto.Amount,*/ /*Description = dto.Description*/ });
            await _db.SaveChangesAsync();
            return Ok("Jeton eklendi.");
        }
    }
} 