using LotteryPredictionApi.DTOs;

namespace LotteryPredictionApi.Services
{
    public interface IPredictionService
    {
        Task<PredictionDto> CreatePredictionAsync(CreatePredictionDto createPredictionDto);
        Task<List<PredictionDto>> GetUserPredictionsAsync(int userId);
        Task<List<PredictionDto>> GetMatchPredictionsAsync(int matchId);
        Task CalculatePointsForMatchAsync(int matchId);
        Task<List<LeaderboardDto>> GetLeaderboardAsync();
    }
}