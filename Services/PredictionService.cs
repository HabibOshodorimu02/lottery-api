using Microsoft.EntityFrameworkCore;
using LotteryPredictionApi.Data;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Models;

namespace LotteryPredictionApi.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly LotteryDbContext _context;

        public PredictionService(LotteryDbContext context)
        {
            _context = context;
        }

        public async Task<PredictionDto> CreatePredictionAsync(CreatePredictionDto createPredictionDto)
        {
            // Validate match exists and is not completed
            var match = await _context.Matches.FindAsync(createPredictionDto.MatchId);
            if (match == null)
                throw new ArgumentException("Match not found");

            if (match.IsCompleted)
                throw new InvalidOperationException("Cannot predict on completed matches");

            // Check if user already has a prediction for this match
            var existingPrediction = await _context.Predictions
                .FirstOrDefaultAsync(p => p.UserId == createPredictionDto.UserId && p.MatchId == createPredictionDto.MatchId);

            if (existingPrediction != null)
                throw new InvalidOperationException("User already has a prediction for this match");

            var prediction = new Prediction
            {
                UserId = createPredictionDto.UserId,
                MatchId = createPredictionDto.MatchId,
                PredictedOutcome = createPredictionDto.PredictedOutcome,
                PredictedHomeScore = createPredictionDto.PredictedHomeScore,
                PredictedAwayScore = createPredictionDto.PredictedAwayScore
            };

            _context.Predictions.Add(prediction);

            // Update user prediction count
            var user = await _context.Users.FindAsync(createPredictionDto.UserId);
            if (user != null)
            {
                user.NumberOfPredictions++;
                user.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return await GetPredictionDtoAsync(prediction.Id);
        }

        public async Task<List<PredictionDto>> GetUserPredictionsAsync(int userId)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Include(p => p.Match)
                    .ThenInclude(m => m.HomeTeam)
                .Include(p => p.Match)
                    .ThenInclude(m => m.AwayTeam)
                .Where(p => p.UserId == userId)
                .Select(p => new PredictionDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    MatchId = p.MatchId,
                    PredictedOutcome = p.PredictedOutcome,
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    PointsEarned = p.PointsEarned,
                    UserName = $"{p.User.FirstName} {p.User.LastName}",
                    MatchDetails = $"{p.Match.HomeTeam.TeamName} vs {p.Match.AwayTeam.TeamName}"
                })
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        public async Task<List<PredictionDto>> GetMatchPredictionsAsync(int matchId)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Include(p => p.Match)
                    .ThenInclude(m => m.HomeTeam)
                .Include(p => p.Match)
                    .ThenInclude(m => m.AwayTeam)
                .Where(p => p.MatchId == matchId)
                .Select(p => new PredictionDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    MatchId = p.MatchId,
                    PredictedOutcome = p.PredictedOutcome,
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    PointsEarned = p.PointsEarned,
                    UserName = $"{p.User.FirstName} {p.User.LastName}",
                    MatchDetails = $"{p.Match.HomeTeam.TeamName} vs {p.Match.AwayTeam.TeamName}"
                })
                .OrderByDescending(p => p.PointsEarned)
                .ToListAsync();
        }

        public async Task CalculatePointsForMatchAsync(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.Predictions)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null || !match.IsCompleted)
                return;

            foreach (var prediction in match.Predictions)
            {
                int points = 0;

                // Check for correct score (10 points)
                if (prediction.PredictedHomeScore == match.HomeScore &&
                    prediction.PredictedAwayScore == match.AwayScore)
                {
                    points = 10;
                }
                else
                {
                    // Check for correct outcome
                    var actualResult = match.Result;
                    if (prediction.PredictedOutcome == actualResult)
                    {
                        points = actualResult switch
                        {
                            "HomeWin" => 2,
                            "AwayWin" => 3,
                            "Draw" => 5,
                            _ => 0
                        };
                    }
                }

                prediction.PointsEarned = points;

                // Update user total points
                prediction.User.TotalPoints += points;
                prediction.User.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<LeaderboardDto>> GetLeaderboardAsync()
        {
            var leaderboard = await _context.Users
                .OrderByDescending(u => u.TotalPoints)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new LeaderboardDto
                {
                    FullName = $"{u.FirstName} {u.LastName}",
                    Email = u.Email,
                    TotalPoints = u.TotalPoints,
                    NumberOfPredictions = u.NumberOfPredictions
                })
                .ToListAsync();

            // Add rankings
            for (int i = 0; i < leaderboard.Count; i++)
            {
                leaderboard[i].Rank = i + 1;
            }

            return leaderboard;
        }

        private async Task<PredictionDto> GetPredictionDtoAsync(int predictionId)
        {
            return await _context.Predictions
                .Include(p => p.User)
                .Include(p => p.Match)
                    .ThenInclude(m => m.HomeTeam)
                .Include(p => p.Match)
                    .ThenInclude(m => m.AwayTeam)
                .Where(p => p.Id == predictionId)
                .Select(p => new PredictionDto
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    MatchId = p.MatchId,
                    PredictedOutcome = p.PredictedOutcome,
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    PointsEarned = p.PointsEarned,
                    UserName = $"{p.User.FirstName} {p.User.LastName}",
                    MatchDetails = $"{p.Match.HomeTeam.TeamName} vs {p.Match.AwayTeam.TeamName}"
                })
                .FirstAsync();
        }
    }
}