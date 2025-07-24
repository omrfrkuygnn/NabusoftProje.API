using Microsoft.EntityFrameworkCore;
using Event.Models; // Eğer entity classların bu klasördeyse

namespace Event.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Senin event ile ilgili tablolar
        public DbSet<EventItem> EventItem { get; set; }
        public DbSet<Rule> Rule { get; set; }
        public DbSet<EventRule> EventRule { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<EventRequest> EventRequests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }

        // Lokasyon tabloları
        public virtual DbSet<Csbm> Csbms { get; set; }
        public virtual DbSet<Ilceler> Ilcelers { get; set; }
        public virtual DbSet<Iller> Illers { get; set; }
        public virtual DbSet<Mahalleler> Mahallelers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EventRule için composite key
            modelBuilder.Entity<EventRule>()
                .HasKey(er => new { er.EventId, er.RuleId, er.GivenBy });

            // Scaffold'tan gelen ayarlar
            modelBuilder.Entity<Csbm>(entity =>
            {
                entity.HasNoKey().ToTable("csbms");

                entity.Property(e => e.BilesenName).HasMaxLength(250).HasColumnName("bilesenName");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IlId).HasColumnName("il_id");
                entity.Property(e => e.IlceId).HasColumnName("ilce_id");
                entity.Property(e => e.MahalleId).HasColumnName("mahalle_id");
                entity.Property(e => e.Name).HasMaxLength(250).HasColumnName("name");
            });

            modelBuilder.Entity<Ilceler>(entity =>
            {
                entity.HasNoKey().ToTable("ilceler");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IlId).HasColumnName("il_id");
                entity.Property(e => e.KimlikNo).HasColumnName("kimlikNo");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
            });

            modelBuilder.Entity<Iller>(entity =>
            {
                entity.HasNoKey().ToTable("iller");

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasMaxLength(50).HasColumnName("name");
                entity.Property(e => e.Plaka).HasColumnName("plaka");
            });

            modelBuilder.Entity<Mahalleler>(entity =>
            {
                entity.HasNoKey().ToTable("mahalleler");

                entity.Property(e => e.BilesenName).HasMaxLength(250).HasColumnName("bilesenName");
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.IlId).HasColumnName("il_id");
                entity.Property(e => e.IlceId).HasColumnName("ilce_id");
                entity.Property(e => e.KimlikNo).HasColumnName("kimlikNo");
                entity.Property(e => e.Name).HasMaxLength(100).HasColumnName("name");
            });
        }
    }
}
