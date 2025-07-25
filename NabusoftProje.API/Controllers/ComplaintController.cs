using EtkinlikKatilimApi.Data;
using EtkinlikKatilimApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ComplaintController : ControllerBase
{
    private readonly EventDbContext _context;

    public ComplaintController(EventDbContext context)
    {
        _context = context;
    }

    // Kullanıcının şikayet oluşturması
    [HttpPost]
    public IActionResult CreateComplaint([FromBody] ComplaintCreateDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var complaint = new Complaint
        {
            UserId = userId,
            EventId = dto.EventId,
            Message = dto.Message,
            Reason = dto.Reason, // yeni alan
            Status = "Bekliyor",
            CreatedAt = DateTime.Now
        };

        _context.Complaints.Add(complaint);
        _context.SaveChanges();

        return Ok("Şikayetiniz alınmıştır. Teşekkür ederiz.");
    }
    [HttpGet("admin")]
    [Authorize(Roles = "Admin")]
    public IActionResult GetAllComplaints()
    {
        var complaints = _context.Complaints
            .Select(c => new
            {
                c.Id,
                c.Message,
                c.Status,
                c.CreatedAt,
                EventTitle = c.Event.Title,
                UserName = c.User.UserName
            })
            .ToList();

        return Ok(complaints);
    }
    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateComplaintStatus(int id, [FromBody] ComplaintStatusUpdateDto dto)
    {
        var complaint = await _context.Complaints
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (complaint == null)
            return NotFound("Şikayet bulunamadı.");

        complaint.Status = dto.NewStatus;
        await _context.SaveChangesAsync();

        // Kullanıcıya bildirim gönder
        var notification = new Notification
        {
            UserId = complaint.UserId,
            Message = $"Şikayetiniz '{dto.NewStatus}' olarak güncellendi.",
            CreatedAt = DateTime.Now,
            IsRead = false
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        return Ok("Şikayet durumu güncellendi ve kullanıcıya bildirim gönderildi.");
    }


}
