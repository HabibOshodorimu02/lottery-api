using System;
using System.Linq;
using System.Threading.Tasks;
using LotteryPredictionApi.Models;

namespace LotteryPredictionApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedDataAsync(LotteryDbContext context)
        {
            // Ensure DB created (safe for local/dev)
            await context.Database.EnsureCreatedAsync();

            // Seed Teams
            if (!context.Teams.Any())
            {
                var teams = new[]
                {
                    new Team { TeamName = "Manchester United" },
                    new Team { TeamName = "Manchester City" },
                    new Team { TeamName = "Liverpool" },
                    new Team { TeamName = "Chelsea" },
                    new Team { TeamName = "Arsenal" },
                    new Team { TeamName = "Tottenham" }
                };
                context.Teams.AddRange(teams);
                await context.SaveChangesAsync();
            }

            // Seed Users (only if none)
            if (!context.Users.Any())
            {
                var users = new[]
                {
                    new User { FirstName = "John", LastName = "Doe", Email = "john.doe@email.com", Password = BCrypt.Net.BCrypt.HashPassword("password123") },
                    new User { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@email.com", Password = BCrypt.Net.BCrypt.HashPassword("password123")}
                };

                context.Users.AddRange(users);
                await context.SaveChangesAsync();

                // create prediction stats rows
                foreach (var u in users)
                {
                    context.PredictionStats.Add(new PredictionStats
                    {
                        UserId = u.Id,
                        TotalPredictions = 0,
                        CorrectOutcomes = 0,
                        CorrectScores = 0,
                        TotalPoints = 0,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();
            }

            // Seed Matches (10) if none
            if (!context.Matches.Any())
            {
                var rnd = new Random();
                var teams = context.Teams.ToList();
                for (int i = 0; i < 10; i++)
                {
                    var home = teams[rnd.Next(teams.Count)];
                    Team away;
                    do { away = teams[rnd.Next(teams.Count)]; } while (away.Id == home.Id);

                    context.Matches.Add(new Match
                    {
                        HomeTeamId = home.Id,
                        AwayTeamId = away.Id,
                        MatchDate = DateTime.UtcNow.AddDays(rnd.Next(-7, 30)),
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow
                    });
                }
                await context.SaveChangesAsync();
            }
        }
    }
}
