using Microsoft.AspNetCore.Mvc;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Services;

namespace LotteryPredictionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionsController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost]
        public async Task<ActionResult<PredictionDto>> CreatePrediction(CreatePredictionDto createPredictionDto)
        {
            try
            {
                var prediction = await _predictionService.CreatePredictionAsync(createPredictionDto);
                return CreatedAtAction(nameof(GetUserPredictions), new { userId = prediction.UserId }, prediction);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<PredictionDto>>> GetUserPredictions(int userId)
        {
            var predictions = await _predictionService.GetUserPredictionsAsync(userId);
            return Ok(predictions);
        }

        [HttpGet("match/{matchId}")]
        public async Task<ActionResult<List<PredictionDto>>> GetMatchPredictions(int matchId)
        {
            var predictions = await _predictionService.GetMatchPredictionsAsync(matchId);
            return Ok(predictions);
        }

        [HttpPost("calculate-points/{matchId}")]
        public async Task<IActionResult> CalculatePoints(int matchId)
        {
            await _predictionService.CalculatePointsForMatchAsync(matchId);
            return Ok("Points calculated successfully");
        }
    }
}