using Microsoft.EntityFrameworkCore;

namespace BasketballScores
{
    // EF Core DbContext
    public class BasketballDbContext : DbContext
    {
        public BasketballDbContext(DbContextOptions<BasketballDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerGameStat> PlayerGameStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.JerseyNumber).IsRequired();
                entity.Property(e => e.Position).HasMaxLength(50);
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            });

            // Game configuration
            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GameDate).IsRequired();
                entity.Property(e => e.OpponentTeam).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.IsCompleted).IsRequired().HasDefaultValue(false);
                entity.Ignore(e => e.Result);
                entity.Ignore(e => e.ScoreDisplay);
            });

            // PlayerGameStat configuration
            modelBuilder.Entity<PlayerGameStat>(entity =>
            {
                entity.ToTable("PlayerGameStats");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.GameId).IsRequired();
                entity.Property(e => e.PlayerId).IsRequired();
                entity.Property(e => e.Points).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Errors).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Assists).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.Rebounds).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.MinutesPlayed).IsRequired().HasDefaultValue(0);

                // Relationships
                entity.HasOne(e => e.Game)
                      .WithMany(g => g.PlayerGameStats)
                      .HasForeignKey(e => e.GameId)
                      .HasConstraintName("FK_PGS_Games")
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Player)
                      .WithMany(p => p.PlayerGameStats)
                      .HasForeignKey(e => e.PlayerId)
                      .HasConstraintName("FK_PGS_Players")
                      .OnDelete(DeleteBehavior.Cascade);

                // Unique constraint
                entity.HasIndex(e => new { e.GameId, e.PlayerId })
                      .IsUnique()
                      .HasDatabaseName("UC_PlayerGameStats");

                // Ignore denormalized properties
                entity.Ignore(e => e.PlayerName);
                entity.Ignore(e => e.JerseyNumber);
            });
        }
    }

    }
