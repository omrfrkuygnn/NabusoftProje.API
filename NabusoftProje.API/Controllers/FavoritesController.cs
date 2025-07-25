using EtkinlikKatilimApi.Data;
using EtkinlikKatilimApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using EtkinlikKatilimApi.ViewModels;

namespace EtkinlikKatilimApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly EventDbContext _context;

        public FavoritesController(EventDbContext context)
        {
            _context = context;
        }

        [HttpPost("ekle")]
        public async Task<IActionResult> AddToFavorites([FromBody] int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var alreadyFav = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (alreadyFav != null)
                return BadRequest("Zaten favorilerde.");

            var favorite = new Favorite
            {
                UserId = userId,
                EventId = eventId
            };

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok("Favorilere eklendi.");
        }

        [HttpDelete("cikar")]
        public async Task<IActionResult> RemoveFromFavorites([FromQuery] int eventId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.EventId == eventId);

            if (favorite == null)
                return NotFound("Favoride bulunamadı.");

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return Ok("Favorilerden çıkarıldı.");
        }

        [HttpGet("listele")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorites = await _context.Favorites
                .Include(f => f.Event)
                .Where(f => f.UserId == userId)
                .Select(f => new FavoriteViewModel
                {
                    EventId = f.EventId,
                    Event = new EventViewModel
                    {
                        EventId = f.Event.EventId,
                        Title = f.Event.Title,
                        Date = f.Event.Date,
                        Description = f.Event.Description // varsa
                    }
                })
                .ToListAsync();

            return Ok(favorites);
        }
    }
}
