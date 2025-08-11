using System.ComponentModel.DataAnnotations;

namespace LotteryPredictionApi.Models
{
    public class PredictionStats
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public int TotalPredictions { get; set; } = 0;
        public int CorrectOutcomes { get; set; } = 0;
        public int CorrectScores { get; set; } = 0;
        public int HomeWinPredictions { get; set; } = 0;
        public int AwayWinPredictions { get; set; } = 0;
        public int DrawPredictions { get; set; } = 0;
        public int TotalPoints { get; set; } = 0;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public virtual User User { get; set; } = null!;
    }
}