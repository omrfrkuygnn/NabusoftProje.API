using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NabusoftProje.API.Data;
using NabusoftProje.API.Models;
using System.Security.Claims;

namespace NabusoftProje.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipationController : ControllerBase
    {
        private readonly EventDbContext _context;

        public ParticipationController(EventDbContext context)
        {
            _context = context;
        }

        // POST: api/participation/talepgonder
        [HttpPost("talepgonder")]
        public async Task<IActionResult> TalepGonder([FromBody] ParticipationRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID alınamadı.");
            }
            var etkinlik = await _context.Events.FindAsync(request.EventId);
            if (etkinlik == null)
                return NotFound("Etkinlik bulunamadı.");

            var mevcutTalep = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.UserId == userId && p.EventId == request.EventId && p.IsApproved != false);
            if (mevcutTalep != null)
                return BadRequest("Bu etkinlik için zaten katılım talebiniz mevcut.");

            var talep = new EventParticipation
            {
                UserId = userId,
                EventId = request.EventId,
                RequestDate = DateTime.Now,
                IsApproved = null,
                RejectReason = null
            };

            _context.EventParticipations.Add(talep);
            await _context.SaveChangesAsync();

            // Kapasite dolduysa organizatöre bildirim gönder
            var approvedCount = await _context.EventParticipations
                .CountAsync(p => p.EventId == etkinlik.EventId && p.IsApproved == true);

            if (approvedCount >= etkinlik.Capacity)
            {
                var bildirim = new Notification
                {
                    EventId = etkinlik.EventId,
                    UserId = etkinlik.OrganizerId,
                    Message = $"Etkinlik '{etkinlik.Title}' kontenjan sınırına ulaştı.",
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(bildirim);
                await _context.SaveChangesAsync();
            }

            return Ok("Katılım talebiniz iletildi.");
        }

        // GET: api/participation/etkinlige-katilim-talepleri?eventId=5
        [HttpGet("etkinlige-katilim-talepleri")]
        public async Task<IActionResult> EtkinligeKatilimTalepleri([FromQuery] int eventId)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID alınamadı.");
            }
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Organizer")
                return Forbid("Bu işlem için yetkiniz yok.");

            var etkinlik = await _context.Events
                .Include(e => e.Participations)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.EventId == eventId && e.OrganizerId == userId);

            if (etkinlik == null)
                return NotFound("Etkinlik bulunamadı veya size ait değil.");

            var talepler = etkinlik.Participations.Select(p => new
            {
                p.Id,
                KullaniciAdi = p.User.UserName,
                p.RequestDate,
                p.IsApproved,
                p.RejectReason
            });

            return Ok(talepler);
        }

        // POST: api/participation/onayla
        [HttpPost("onayla")]
        public async Task<IActionResult> Onayla([FromBody] ApprovalRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID alınamadı.");
            }
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Organizer")
                return Forbid();

            var talep = await _context.EventParticipations
                .Include(p => p.Event).Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == request.ParticipationId);

            if (talep == null)
                return NotFound("Katılım talebi bulunamadı.");

            if (talep.Event.OrganizerId != userId)
                return Forbid("Bu talebi onaylama yetkiniz yok.");

            var onayliSayisi = await _context.EventParticipations
                .CountAsync(p => p.EventId == talep.EventId && p.IsApproved == true);

            if (onayliSayisi >= talep.Event.Capacity)
                return BadRequest("Etkinliğin kontenjanı dolmuş.");

            talep.IsApproved = true;
            talep.RejectReason = null;

            // Katılımcıya bildirim gönder
            var bildirim = new Notification
            {
                EventId = talep.EventId,
                UserId = talep.UserId,
                Message = $"Etkinlik '{talep.Event.Title}' katılım talebiniz onaylandı.",
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(bildirim);

            await _context.SaveChangesAsync();
            return Ok("Katılım talebi onaylandı.");
        }

        // POST: api/participation/reddet
        [HttpPost("reddet")]
        public async Task<IActionResult> Reddet([FromBody] RejectionRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID alınamadı.");
            }
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Organizer")
                return Forbid();

            var talep = await _context.EventParticipations
                .Include(p => p.Event).Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == request.ParticipationId);

            if (talep == null)
                return NotFound("Katılım talebi bulunamadı.");

            if (talep.Event.OrganizerId != userId)
                return Forbid("Bu talebi reddetme yetkiniz yok.");

            talep.IsApproved = false;
            talep.RejectReason = request.RejectReason;

            // Katılımcıya bildirim gönder
            var bildirim = new Notification
            {
                EventId = talep.EventId,
                UserId = talep.UserId,
                Message = $"Etkinlik '{talep.Event.Title}' katılım talebiniz reddedildi. Sebep: {request.RejectReason}",
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(bildirim);

            await _context.SaveChangesAsync();
            return Ok("Katılım talebi reddedildi.");
        }

        // POST: api/participation/iptalet
        [HttpPost("iptalet")]
        public async Task<IActionResult> IptalEt([FromBody] int eventId)
        {
            var userIdString = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kullanıcı ID alınamadı.");
            }
            var participation = await _context.EventParticipations
                .FirstOrDefaultAsync(p => p.UserId == userId && p.EventId == eventId);
            if (participation == null)
                return NotFound("Katılım talebiniz bulunamadı.");

            _context.EventParticipations.Remove(participation);
            await _context.SaveChangesAsync();
            return Ok("Katılım talebiniz iptal edildi.");
        }
    }
}
