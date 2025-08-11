using Microsoft.AspNetCore.Mvc;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Services;

namespace LotteryPredictionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public LeaderboardController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet]
        public async Task<ActionResult<List<LeaderboardDto>>> GetLeaderboard()
        {
            var leaderboard = await _predictionService.GetLeaderboardAsync();
            return Ok(leaderboard);
        }
    }
}