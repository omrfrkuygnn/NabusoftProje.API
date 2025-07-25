using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NabusoftProje.API.Models;
using NabusoftProje.API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public EventsController(AppDbContext db)
        {
            _db = db;
        }

        // GET: /api/events
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _db.Events.ToListAsync();
            return Ok(events);
        }

        // POST: /api/events
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var @event = new Event
            {
                Name = dto.Name,
                Description = dto.Description,
                Date = dto.Date,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };
            _db.Events.Add(@event);
            await _db.SaveChangesAsync();
            return Ok(@event);
        }

        // GET: /api/events/mine
        [Authorize]
        [HttpGet("mine")]
        public async Task<IActionResult> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var created = await _db.Events.Where(e => e.CreatedBy == userId).ToListAsync();
            var joined = await _db.EventParticipants.Where(p => p.UserId == userId).ToListAsync();
            return Ok(new { created, joined });
        }

        // POST: /api/events/{id}/join
        [Authorize]
        [HttpPost("{id}/join")]
        public async Task<IActionResult> Join(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(ClaimTypes.Name);
            var @event = await _db.Events.FindAsync(id);
            if (@event == null) return NotFound();
            _db.EventParticipants.Add(new EventParticipant { EventId = id, UserId = userId });
            await _db.SaveChangesAsync();
            return Ok("Etkinliğe katılım başarılı.");
        }
    }
} 