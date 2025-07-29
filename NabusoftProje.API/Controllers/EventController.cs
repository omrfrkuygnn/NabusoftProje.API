using LocationManagement.API.Data;
using LocationManagement.API.Models;
using LocationManagement.API.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Tüm etkinlikleri getir - GET: api/Event
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var events = await _context.Events
                .Include(e => e.City)
                .Include(e => e.District)
                .Include(e => e.Neighborhood)
                .Include(e => e.Quarter)
                .Select(e => new EventViewModel
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Date = e.Date,
                    IsFree = e.IsFree,
                    CityName = e.City.CityName,
                    DistrictName = e.District.DistrictName,
                    NeighborhoodName = e.Neighborhood.NeighborhoodName,
                    QuarterName = e.Quarter.QuarterName
                })
                .ToListAsync();

            return Ok(events);
        }

        // ✅ Yeni etkinlik oluştur - POST: api/Event
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventCreateApiViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newEvent = new Event
            {
                Title = model.Title,
                Description = model.Description,
                Date = model.Date,
                IsFree = model.IsFree,
                CityId = model.CityId,
                DistrictId = model.DistrictId,
                NeighborhoodId = model.NeighborhoodId,
                QuarterId = model.QuarterId
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                newEvent.EventId,
                newEvent.Title,
                newEvent.Date,
                newEvent.CityId
            });
        }
    }
}