using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LotteryPredictionApi.Data;
using LotteryPredictionApi.DTOs;
using LotteryPredictionApi.Models;

namespace LotteryPredictionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatchesController : ControllerBase
    {
        private readonly LotteryDbContext _context;

        public MatchesController(LotteryDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<MatchDto>>> GetMatches()
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam.TeamName,
                    AwayTeam = m.AwayTeam.TeamName,
                    MatchDate = m.MatchDate,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    IsCompleted = m.IsCompleted,
                    Result = m.Result
                })
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return Ok(matches);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MatchDto>> GetMatch(int id)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.Id == id)
                .Select(m => new MatchDto
                {
                    Id = m.Id,
                    HomeTeam = m.HomeTeam.TeamName,
                    AwayTeam = m.AwayTeam.TeamName,
                    MatchDate = m.MatchDate,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore,
                    IsCompleted = m.IsCompleted,
                    Result = m.Result
                })
                .FirstOrDefaultAsync();

            if (match == null)
                return NotFound();

            return Ok(match);
        }

        [HttpPut("{id}/result")]
        public async Task<IActionResult> UpdateMatchResult(int id, [FromBody] UpdateMatchResultDto dto)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
                return NotFound();

            match.HomeScore = dto.HomeScore;
            match.AwayScore = dto.AwayScore;
            match.IsCompleted = true;

            await _context.SaveChangesAsync();

            return Ok("Match result updated successfully");
        }
    }

    public class UpdateMatchResultDto
    {
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }
}