using Event.Data;
using Event.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Event.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventItemController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromBody] EventItem eventItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int userId = 3;
            string packageType = "Paket1"; 

           
            if (string.IsNullOrWhiteSpace(eventItem.EventTitle) ||
                string.IsNullOrWhiteSpace(eventItem.EventDescription) ||
                string.IsNullOrWhiteSpace(eventItem.EventType) ||
                string.IsNullOrWhiteSpace(eventItem.EventCategory) ||
                string.IsNullOrEmpty(eventItem.CityName) ||
                string.IsNullOrEmpty(eventItem.DistrictName) ||
                string.IsNullOrEmpty(eventItem.NeighborhoodName) ||
                string.IsNullOrEmpty(eventItem.StreetName) ||
                string.IsNullOrWhiteSpace(eventItem.EventLocation) ||
                string.IsNullOrWhiteSpace(eventItem.EventImg) ||
                eventItem.EventStartDate == default ||
                eventItem.EventEndDate == default)
            {
                return BadRequest("Tüm alanlar zorunludur.");
            }

            if (eventItem.RuleIds == null || !eventItem.RuleIds.Any())
            {
                return BadRequest("En az bir kural seçilmelidir.");
            }

            bool isPaid = eventItem.EventPrice > 0;
            var oneMonthAgo = DateTime.Now.AddMonths(-1);

            var userEventsLastMonth = await _context.EventItem
                .Where(e => e.UserId == userId && e.CreatedAt >= oneMonthAgo)
                .ToListAsync();

            var freeCount = userEventsLastMonth.Count(e => e.EventPrice == 0);
            var paidCount = userEventsLastMonth.Count(e => e.EventPrice > 0);

            if (packageType == "Free")
            {
                if (isPaid)
                    return BadRequest("Ücretsiz pakette ücretli etkinlik oluşturamazsınız.");

                if (freeCount >= 1)
                    return BadRequest("Bu ay zaten bir ücretsiz etkinlik oluşturdunuz.");

                if (eventItem.EventPrice != 0)
                    return BadRequest("Ücretsiz etkinlik için fiyat 0 olmalıdır.");
            }
            else if (packageType == "Paket1")
            {
                if (isPaid && paidCount >= 2)
                    return BadRequest("Paket 1 ile ayda en fazla 2 ücretli etkinlik oluşturabilirsiniz.");

                if (!isPaid && eventItem.EventPrice != 0)
                    return BadRequest("Ücretsiz etkinlik için fiyat 0 olmalıdır.");
            }
            else if (packageType == "Paket2")
            {
                if (!isPaid && eventItem.EventPrice != 0)
                    return BadRequest("Ücretsiz etkinlik için fiyat 0 olmalıdır.");
            }

            eventItem.UserId = userId;
            eventItem.Status = "Onay Bekliyor";
            eventItem.CreatedAt = DateTime.Now;

            _context.EventItem.Add(eventItem);
            await _context.SaveChangesAsync();

            foreach (var ruleId in eventItem.RuleIds)
            {
                var eventRule = new EventRule
                {
                    EventId = eventItem.EventId,
                    RuleId = ruleId,
                    GivenBy = userId,
                    GivenAt = DateTime.Now
                };
                _context.EventRule.Add(eventRule);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Etkinlik oluşturuldu, onay bekleniyor", eventId = eventItem.EventId });
        }
        [HttpGet("created-by-me")]
        public async Task<IActionResult> GetMyEvents()
        {
            int userId = 3; // Giriş yapılmadığı için sabit değer

            var myEvents = await _context.EventItem
                .Where(e => e.UserId == userId && !e.IsDeleted)  // ❗ Burada silinmemişleri filtreledik
                .Select(e => new
                {
                    e.EventId,
                    e.EventTitle,
                    e.EventDescription,
                    e.EventStartDate,
                    e.EventEndDate,
                    e.EventType,
                    e.EventCategory,
                    e.CityName,
                    e.DistrictName,
                    e.NeighborhoodName,
                    e.StreetName,
                    e.EventLocation,
                    e.Status,
                    e.CancelledAt,
                })
                .ToListAsync();

            return Ok(myEvents);
        }

        [HttpGet("{eventId:int}")]
        public async Task<IActionResult> GetEventById(int eventId)
        {
            var eventItem = await _context.EventItem.FindAsync(eventId);
            if (eventItem == null)
                return NotFound("Etkinlik bulunamadı.");

            // Kurallarını da getir
            var rules = await _context.EventRule
                .Where(er => er.EventId == eventId)
                .Include(er => er.Rule)
                .Select(er => er.Rule.RuleDescription)
                .ToListAsync();

            return Ok(new
            {
                eventItem.EventId,
                eventItem.EventTitle,
                eventItem.EventDescription,
                eventItem.EventType,
                eventItem.EventCategory,
                eventItem.EventStartDate,
                eventItem.EventEndDate,
                eventItem.CityName,
                eventItem.DistrictName,
                eventItem.NeighborhoodName,
                eventItem.StreetName,
                eventItem.EventLocation,
                eventItem.EventImg,
                eventItem.EventQuota,
                eventItem.EventPrice,
                eventItem.IsPublic,
                eventItem.Status,
                eventItem.CancelledAt,
                Rules = rules
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] EventItem updatedEvent)
        {
            var existing = await _context.EventItem.FindAsync(id);
            if (existing == null) return NotFound();

            if ((existing.EventStartDate - DateTime.Now).TotalHours < 24)
                return BadRequest("Etkinlik başlamasına 24 saatten az kaldı, güncelleyemezsiniz.");

            existing.EventTitle = updatedEvent.EventTitle;
            existing.EventDescription = updatedEvent.EventDescription;
            existing.EventType = updatedEvent.EventType;
            existing.EventCategory = updatedEvent.EventCategory;
            existing.EventStartDate = updatedEvent.EventStartDate;
            existing.EventEndDate = updatedEvent.EventEndDate;
            existing.CityName = updatedEvent.CityName;
            existing.DistrictName = updatedEvent.DistrictName;
            existing.NeighborhoodName = updatedEvent.NeighborhoodName;
            existing.StreetName = updatedEvent.StreetName;
            existing.EventLocation = updatedEvent.EventLocation;
            existing.EventImg = updatedEvent.EventImg;
            existing.EventQuota = updatedEvent.EventQuota;
            existing.EventPrice = updatedEvent.EventPrice;
            existing.RuleIds = updatedEvent.RuleIds;

            existing.Status = "Onay Bekliyor";
            await _context.SaveChangesAsync();
            return Ok(existing);
        }
        [HttpPut("cancel/{id}")]
        public async Task<IActionResult> CancelEvent(int id)
        {
            var eventItem = await _context.EventItem.FindAsync(id);
            if (eventItem == null) return NotFound();

            if ((eventItem.EventStartDate - DateTime.Now).TotalHours < 24)
                return BadRequest("Etkinlik başlamasına 24 saatten az kaldı, iptal edemezsiniz.");
            eventItem.Status = "İptal Bekliyor";
            eventItem.CancelledAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return Ok("İptal için onay bekleniyor.");
        }
        [HttpPost("undo-cancel/{id}")]
        public async Task<IActionResult> UndoCancelEvent(int id)
        {
            var eventItem = await _context.EventItem.FindAsync(id);
            if (eventItem == null || eventItem.Status != "İptal Edildi")
                return BadRequest("Geri alınacak uygun bir etkinlik yok.");

            if (eventItem.CancelledAt == null || (DateTime.Now - eventItem.CancelledAt.Value).TotalMinutes > 60)
                return BadRequest("İptal üzerinden 1 saat geçti, geri alınamaz.");

            eventItem.Status = "Aktif";
            eventItem.CancelledAt = null;

            var userIds = await _context.EventRequests
                .Where(r => r.EventId == id)
                .Select(r => r.UserId)
                .ToListAsync();

            foreach (var uid in userIds.Distinct())
            {
                _context.Notifications.Add(new Notification
                {
                    UserId = uid,
                    Message = $"'{eventItem.EventTitle}' etkinliği tekrar aktif hale getirildi.",
                    CreatedAt = DateTime.Now,
                    IsRead = false
                });
            }

            await _context.SaveChangesAsync();
            return Ok("Etkinlik geri alındı.");
        }
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveEvent(int id)
        {
            var eventItem = await _context.EventItem.FindAsync(id);
            if (eventItem == null) return NotFound();

            if (eventItem.Status == "İptal Bekliyor")
            {
                eventItem.Status = "İptal Edildi";

                var userIds = await _context.EventRequests
                    .Where(r => r.EventId == id)
                    .Select(r => r.UserId)
                    .ToListAsync();

                foreach (var uid in userIds.Distinct())
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = uid,
                        Message = $"'{eventItem.EventTitle}' etkinliği iptal edildi.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    });
                }

                _context.Notifications.Add(new Notification
                {
                    UserId = eventItem.UserId,
                    Message = $"'{eventItem.EventTitle}' adlı etkinliğiniz iptal edildi.",
                    CreatedAt = DateTime.Now
                });
            }
            else if (eventItem.Status == "Onay Bekliyor")
            {
                eventItem.Status = "Aktif";

                _context.Notifications.Add(new Notification
                {
                    UserId = eventItem.UserId,
                    Message = $"'{eventItem.EventTitle}' adlı etkinliğiniz onaylandı.",
                    CreatedAt = DateTime.Now
                });

                var userIds = await _context.EventRequests
                    .Where(r => r.EventId == id)
                    .Select(r => r.UserId)
                    .ToListAsync();

                foreach (var uid in userIds.Distinct())
                {
                    _context.Notifications.Add(new Notification
                    {
                        UserId = uid,
                        Message = $"'{eventItem.EventTitle}' etkinliği güncellendi.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Etkinlik onaylandı.");
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.EventItem.FindAsync(id);
            if (eventItem == null) return NotFound();

            bool isPast = eventItem.EventEndDate < DateTime.Now;
            bool isCancelled = eventItem.Status == "İptal Edildi";

            if (!isPast && !isCancelled)
                return BadRequest("Yalnızca iptal edilmiş veya süresi geçmiş etkinlikler silinebilir.");

            // ❗ Yumuşak silme uyguluyoruz
            eventItem.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok("Etkinlik başarıyla silindi (görünmez yapıldı).");
        }

        [HttpGet("can-create-event")]
        public async Task<IActionResult> CanCreateEvent(bool isPaid)
        {
            int userId = 3;
            string packageType = "Paket1";

            var oneMonthAgo = DateTime.Now.AddMonths(-1);

            var events = await _context.EventItem
                .Where(e => e.UserId == userId && e.CreatedAt >= oneMonthAgo)
                .ToListAsync();

            var freeCount = events.Count(e => e.EventPrice == 0);
            var paidCount = events.Count(e => e.EventPrice > 0);

            if(packageType == "Free")
            {
                if (isPaid)
                    return BadRequest("Ücretsiz pakette ücretli etkinlik oluşturamazsınız.");
                if (freeCount >= 1)
                    return BadRequest("Bu ay zaten bir ücretsiz etkinlik oluşturdunuz.");
            }
            if(packageType == "Paket1")
            {
                if (isPaid && paidCount >= 2)
                    return BadRequest("Paket 1 ile ayda en fazla 2 ücretli etkinlik oluşturabilirsiniz.");
            }
            return Ok("Etkinlik oluşturabilirsiniz.");
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllEvents()
        {
            var now = DateTime.UtcNow;

            var events = await _context.EventItem
                .Where(e => e.Status == "Aktif" || e.Status == "Süresi Doldu") 
                .Select(e => new
                {
                    Id = e.EventId,
                    Title = e.EventTitle,
                    StartDate = e.EventStartDate,
                    EndDate = e.EventEndDate,
                    Location = e.EventLocation,
                    Img = e.EventImg,
                    Price = e.EventPrice,
                    Status = e.Status
                })
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            var grouped = new
            {
                AktifEtkinlikler = events.Where(e => e.Status == "Aktif"),
                SuresiDolmusEtkinlikler = events.Where(e => e.Status == "Süresi Doldu")
            };

            return Ok(grouped);
        }

        [HttpGet("{id}/questions")]
        public async Task<IActionResult> GetQuestions(int id)
        {
            var questions = await _context.Questions
                .Where(q => q.EventId == id)
                .OrderByDescending(q => q.CreatedAt)
                .Select(q => new
                {
                    q.QuestionId,
                    q.Text,
                    q.Answer,
                    q.CreatedAt
                })
                .ToListAsync();

            return Ok(questions);
        }


        [HttpPost("{id}/questions")]
        public async Task<IActionResult> AskQuestion(int id, [FromBody] string text)
        {
            string userId = "1"; // şimdilik sabit

            var ev = await _context.EventItem.FindAsync(id);
            if (ev == null)
                return NotFound("Etkinlik bulunamadı");

            //if (ev.UserId.ToString() == userId)
            //  return BadRequest("Etkinlik sahibisin, soru soramazsın");

            var question = new Question
            {
                EventId = id,
                Text = text,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await _context.Comments
                .Where(c => c.EventId == id)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.CommentId,
                    c.Text,
                    c.Reply,
                    c.CreatedAt,
                    LikeCount = _context.CommentLikes.Count(l => l.CommentId == c.CommentId)
                })
                .ToListAsync();

            return Ok(comments);
        }


        [HttpPost("{id}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] string text)
        {
            string userId = "1"; // sabit kullanıcı

            var ev = await _context.EventItem.FindAsync(id);
            if (ev == null)
                return NotFound("Etkinlik bulunamadı");

            //if (ev.UserId.ToString() == userId)
            //  return BadRequest("Etkinlik sahibisin, yorum yapamazsın");

            if (ev.EventEndDate > DateTime.UtcNow)
                return BadRequest("Etkinlik henüz bitmemiş, yorum yapılamaz");

            var comment = new Comment
            {
                EventId = id,
                Text = text,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return Ok(comment);
        }
        [HttpPost("{eventId}/questions/{questionId}/answer")]
        public async Task<IActionResult> AnswerQuestion(int eventId, int questionId, [FromBody] string answerText)
        {
            string userId = "3"; // Sabit userId (Etkinlik sahibi)

            var ev = await _context.EventItem.FindAsync(eventId);
            if (ev == null)
                return NotFound("Etkinlik bulunamadı");

            if (ev.UserId.ToString() != userId)
                return BadRequest("Sadece etkinlik sahibi cevap yazabilir");

            var question = await _context.Questions.FindAsync(questionId);
            if (question == null || question.EventId != eventId)
                return NotFound("Soru bulunamadı");

            question.Answer = answerText;
            await _context.SaveChangesAsync();

            return Ok(question);
        }

        [HttpPost("{eventId}/comments/{commentId}/reply")]
        public async Task<IActionResult> ReplyToComment(int eventId, int commentId, [FromBody] string replyText)
        {
            string userId = "3"; // Şimdilik sabit kullanıcı (etkinlik sahibi)

            var ev = await _context.EventItem.FindAsync(eventId);
            if (ev == null)
                return NotFound("Etkinlik bulunamadı");

            if (ev.UserId.ToString() != userId)
                return BadRequest("Sadece etkinlik sahibi cevap yazabilir");

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.EventId != eventId)
                return NotFound("Yorum bulunamadı");

            comment.Reply = replyText;
            await _context.SaveChangesAsync();

            return Ok(comment);
        }
        [HttpPost("{eventId}/comments/{commentId}/like")]
        public async Task<IActionResult> ToggleLike(int eventId, int commentId)
        {
            string userId = "3"; // sabit kullanıcı

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.EventId != eventId)
                return NotFound("Yorum bulunamadı");

            var existing = await _context.CommentLikes
                .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);

            if (existing != null)
            {
                _context.CommentLikes.Remove(existing);
                await _context.SaveChangesAsync();
                return Ok("Beğeni kaldırıldı");
            }
            else
            {
                var like = new CommentLike
                {
                    CommentId = commentId,
                    UserId = userId,
                    LikedAt = DateTime.Now
                };
                _context.CommentLikes.Add(like);

                await _context.SaveChangesAsync();
                return Ok("Beğenildi");
            }
        }
        /*[HttpGet("{eventId}/comments/{commentId}/likes")]
        public async Task<IActionResult> GetLikeCount(int eventId, int commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null || comment.EventId != eventId)
                return NotFound("Yorum bulunamadı");

            int count = await _context.CommentLikes
                .CountAsync(cl => cl.CommentId == commentId);

            return Ok(count);
        }*/

    }
}
