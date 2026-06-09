using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BasketballScores
{
    // EF Core DbContext
    public class BasketballDbContext : DbContext
    {
        public BasketballDbContext() : base()
        {
        }

        public BasketballDbContext(DbContextOptions<BasketballDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<PlayerGameStat> PlayerGameStats { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connStr = ConfigurationManager.ConnectionStrings["BasketballDB"].ConnectionString;
                optionsBuilder.UseSqlServer(connStr);
            }
        }

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
                      .HasName("UC_PlayerGameStats");

                // Ignore denormalized properties
                entity.Ignore(e => e.PlayerName);
                entity.Ignore(e => e.JerseyNumber);
            });
        }
    }

    // Static facade to minimize Web Forms page changes
    public static class Database
    {
        private static BasketballDbContext CreateContext()
        {
            return new BasketballDbContext();
        }

        public static void Initialize()
        {
            using (var context = CreateContext())
            {
                context.Database.EnsureCreated();
            }
        }

        // ── Players ────────────────────────────────────────────────────────────────

        public static List<Player> GetPlayers(bool activeOnly = false)
        {
            using (var context = CreateContext())
            {
                var query = context.Players.AsQueryable();
                if (activeOnly)
                    query = query.Where(p => p.IsActive);
                return query.OrderBy(p => p.JerseyNumber).ToList();
            }
        }

        public static Player GetPlayer(int id)
        {
            using (var context = CreateContext())
            {
                return context.Players.Find(id);
            }
        }

        public static void SavePlayer(Player p)
        {
            using (var context = CreateContext())
            {
                if (p.Id == 0)
                    context.Players.Add(p);
                else
                {
                    context.Players.Attach(p);
                    context.Entry(p).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        public static void DeletePlayer(int id)
        {
            using (var context = CreateContext())
            {
                var player = context.Players.Find(id);
                if (player != null)
                {
                    player.IsActive = false;
                    context.SaveChanges();
                }
            }
        }

        // ── Games ──────────────────────────────────────────────────────────────────

        public static List<Game> GetGames()
        {
            using (var context = CreateContext())
            {
                return context.Games.OrderBy(g => g.GameDate).ToList();
            }
        }

        public static Game GetGame(int id)
        {
            using (var context = CreateContext())
            {
                return context.Games.Find(id);
            }
        }

        public static int SaveGame(Game g)
        {
            using (var context = CreateContext())
            {
                if (g.Id == 0)
                {
                    context.Games.Add(g);
                    context.SaveChanges();
                    return g.Id;
                }
                else
                {
                    context.Games.Attach(g);
                    context.Entry(g).State = EntityState.Modified;
                    context.SaveChanges();
                    return g.Id;
                }
            }
        }

        public static void DeleteGame(int id)
        {
            using (var context = CreateContext())
            {
                var game = context.Games.Include(g => g.PlayerGameStats).FirstOrDefault(g => g.Id == id);
                if (game != null)
                {
                    context.Games.Remove(game);
                    context.SaveChanges();
                }
            }
        }

        // ── PlayerGameStats ────────────────────────────────────────────────────────

        public static List<PlayerGameStat> GetStatsForGame(int gameId)
        {
            using (var context = CreateContext())
            {
                var stats = context.PlayerGameStats
                    .Where(s => s.GameId == gameId)
                    .Include(s => s.Player)
                    .ToList();

                // Populate denormalized properties for UI binding
                foreach (var stat in stats)
                {
                    stat.PlayerName = stat.Player?.Name;
                    stat.JerseyNumber = stat.Player?.JerseyNumber ?? 0;
                }

                return stats.OrderBy(s => s.JerseyNumber).ToList();
            }
        }

        public static void SaveStat(PlayerGameStat s)
        {
            using (var context = CreateContext())
            {
                if (s.Id == 0)
                {
                    // Check for existing stat (unique constraint)
                    var existing = context.PlayerGameStats
                        .FirstOrDefault(ps => ps.GameId == s.GameId && ps.PlayerId == s.PlayerId);

                    if (existing != null)
                    {
                        // Update existing
                        existing.Points = s.Points;
                        existing.Errors = s.Errors;
                        existing.Assists = s.Assists;
                        existing.Rebounds = s.Rebounds;
                        existing.MinutesPlayed = s.MinutesPlayed;
                    }
                    else
                    {
                        // Insert new
                        context.PlayerGameStats.Add(s);
                    }
                }
                else
                {
                    context.PlayerGameStats.Attach(s);
                    context.Entry(s).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        // ── Aggregates ─────────────────────────────────────────────────────────────

        public static List<PlayerSummary> GetPlayerSummaries()
        {
            using (var context = CreateContext())
            {
                var summaries = context.Players
                    .Where(p => p.IsActive)
                    .Select(p => new
                    {
                        Player = p,
                        Stats = p.PlayerGameStats
                    })
                    .ToList()
                    .Select(x => new PlayerSummary
                    {
                        PlayerId = x.Player.Id,
                        Name = x.Player.Name,
                        JerseyNumber = x.Player.JerseyNumber,
                        Position = x.Player.Position,
                        GamesPlayed = x.Stats.Count,
                        AvgPoints = x.Stats.Any() ? x.Stats.Average(s => s.Points) : 0,
                        AvgErrors = x.Stats.Any() ? x.Stats.Average(s => s.Errors) : 0,
                        TotalPoints = x.Stats.Sum(s => s.Points),
                        TotalErrors = x.Stats.Sum(s => s.Errors)
                    })
                    .OrderBy(s => s.JerseyNumber)
                    .ToList();

                return summaries;
            }
        }
    }
}
