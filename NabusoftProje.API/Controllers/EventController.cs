using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EtkinlikKatilimApi.Data;
using System.Security.Claims;
using System.Globalization;

namespace EtkinlikKatilimApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Organizer")] // Sadece organizatörler erişebilir
    public class EventsController : ControllerBase
    {
        private readonly EventDbContext _context;

        public EventsController(EventDbContext context)
        {
            _context = context;
        }

        [HttpGet("ownedevents")]
        public async Task<IActionResult> GetOwnedEvents()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var events = await _context.Events
                .Where(e => e.OrganizerId == userId)
                .Select(e => new
                {
                    e.EventId,
                    e.Title,
                    e.Description,
                    Date = e.Date,
                    e.Capacity,
                    ApprovedParticipants = e.Participations.Count(p => p.IsApproved == true)
                })
                .ToListAsync();

            return Ok(events);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllEvents(
            [FromQuery] string? title,
            [FromQuery] string? category,
            [FromQuery] string? city,
            [FromQuery] DateTime? date)
        {
            var query = _context.Events.Include(e => e.Participations).AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
                query = query.Where(e => e.Title.Contains(title));
            if (!string.IsNullOrWhiteSpace(category))
                query = query.Where(e => e.Category == category);
            if (!string.IsNullOrWhiteSpace(city))
            {
                var cityLower = city.ToLower();
                query = query.Where(e =>
                    e.City != null &&
                    e.City.ToLower().StartsWith(cityLower)
                );
            }
            if (date.HasValue)
                query = query.Where(e => e.Date.Date == date.Value.Date);

            int? userId = null;
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (int.TryParse(userIdString, out int parsedUserId))
                userId = parsedUserId;

            var events = await query
                .Select(e => new EtkinlikKatilimApi.ViewModels.EventViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    City = e.City,
                    Category = e.Category,
                    IsActive = e.IsActive,
                    IsParticipated = userId != null && e.Participations.Any(p => p.UserId == userId),
                    IsExpired = e.Date.Date <= DateTime.Today,
                    Capacity = e.Capacity,
                    ApprovedParticipants = e.Participations.Count(p => p.IsApproved == true),
                    IsFull = e.Participations.Count(p => p.IsApproved == true) >= e.Capacity
                })
                .ToListAsync();

            return Ok(events);
        }

    }
}
