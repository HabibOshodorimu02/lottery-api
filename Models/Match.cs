// Models/Match.cs
using System.ComponentModel.DataAnnotations;

namespace LotteryPredictionApi.Models
{
    public class Match
    {
        public int Id { get; set; }

        [Required]
        public int HomeTeamId { get; set; }

        [Required]
        public int AwayTeamId { get; set; }

        [Required]
        public DateTime MatchDate { get; set; }

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public bool IsCompleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual Team HomeTeam { get; set; } = null!;
        public virtual Team AwayTeam { get; set; } = null!;
        public virtual ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();

        // Computed Property for Match Result
        public string? Result
        {
            get
            {
                if (!IsCompleted || HomeScore == null || AwayScore == null)
                    return null;

                if (HomeScore > AwayScore) return "HomeWin";
                if (AwayScore > HomeScore) return "AwayWin";
                return "Draw";
            }
        }
    }
}