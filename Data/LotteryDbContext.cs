using Microsoft.EntityFrameworkCore;
using LotteryPredictionApi.Models;

namespace LotteryPredictionApi.Data
{
    public class LotteryDbContext : DbContext
    {
        public LotteryDbContext(DbContextOptions<LotteryDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Prediction> Predictions { get; set; }
        public DbSet<PredictionStats> PredictionStats { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasOne(u => u.Stats)
                      .WithOne(s => s.User)
                      .HasForeignKey<PredictionStats>(s => s.UserId);
            });

            // Match Configuration
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasOne(m => m.HomeTeam)
                      .WithMany(t => t.HomeMatches)
                      .HasForeignKey(m => m.HomeTeamId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.AwayTeam)
                      .WithMany(t => t.AwayMatches)
                      .HasForeignKey(m => m.AwayTeamId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Prediction Configuration
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasIndex(p => new { p.UserId, p.MatchId }).IsUnique();

                entity.HasOne(p => p.User)
                      .WithMany(u => u.Predictions)
                      .HasForeignKey(p => p.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(p => p.Match)
                      .WithMany(m => m.Predictions)
                      .HasForeignKey(p => p.MatchId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}