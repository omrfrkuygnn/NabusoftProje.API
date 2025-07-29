using LocationManagement.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LocationManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ Tüm şehirleri alfabetik sırayla getir
        [HttpGet("cities")]
        public async Task<IActionResult> GetCities()
        {
            var cities = await _context.Cities
                .OrderBy(c => c.CityName)
                .Select(c => new SelectListItem
                {
                    Value = c.CityId.ToString(),
                    Text = c.CityName
                })
                .ToListAsync();

            return Ok(cities);
        }

        // ✅ Seçilen şehre ait ilçeleri getir (alfabetik)
        [HttpGet("districts/{cityId}")]
        public async Task<IActionResult> GetDistricts(int cityId)
        {
            var districts = await _context.Districts
                .Where(d => d.CityId == cityId)
                .OrderBy(d => d.DistrictName)
                .Select(d => new SelectListItem
                {
                    Value = d.DistrictId.ToString(),
                    Text = d.DistrictName
                })
                .ToListAsync();

            return Ok(districts);
        }

        // ✅ Seçilen ilçeye ait mahalleleri getir (alfabetik)
        [HttpGet("neighborhoods/{districtId}")]
        public async Task<IActionResult> GetNeighborhoods(int districtId)
        {
            var neighborhoods = await _context.Neighborhoods
                .Where(n => n.DistrictId == districtId)
                .OrderBy(n => n.NeighborhoodName)
                .Select(n => new SelectListItem
                {
                    Value = n.NeighborhoodId.ToString(),
                    Text = n.NeighborhoodName
                })
                .ToListAsync();

            return Ok(neighborhoods);
        }

        // ✅ Seçilen mahalleye ait semtleri getir (alfabetik)
        [HttpGet("quarters/{neighborhoodId}")]
        public async Task<IActionResult> GetQuarters(int neighborhoodId)
        {
            var quarters = await _context.Quarters
                .Where(q => q.NeighborhoodId == neighborhoodId)
                .OrderBy(q => q.QuarterName)
                .Select(q => new SelectListItem
                {
                    Value = q.QuarterId.ToString(),
                    Text = q.QuarterName
                })
                .ToListAsync();

            return Ok(quarters);
        }
    }
}