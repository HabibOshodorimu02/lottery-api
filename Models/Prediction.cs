using System.ComponentModel.DataAnnotations;

namespace LotteryPredictionApi.Models
{
    public class Prediction
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int MatchId { get; set; }

        [Required]
        [StringLength(20)]
        public string PredictedOutcome { get; set; } = string.Empty; // HomeWin, AwayWin, Draw

        public int? PredictedHomeScore { get; set; }
        public int? PredictedAwayScore { get; set; }
        public int PointsEarned { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual User User { get; set; } = null!;
        public virtual Match Match { get; set; } = null!;
    }
}