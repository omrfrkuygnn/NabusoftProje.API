using Event.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event.Controllers
{
    [ApiController]
    [Route("api/rules")]
    public class RuleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RuleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRules()
        {
            var rules = await _context.Rule
                .Select(r => new
                {
                    r.RuleID,
                    r.RuleDescription
                })
                .ToListAsync();

            return Ok(rules);
        }
    }
}
