namespace LotteryPredictionApi.DTOs
{
    public class PredictionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MatchId { get; set; }
        public string PredictedOutcome { get; set; } = string.Empty;
        public int? PredictedHomeScore { get; set; }
        public int? PredictedAwayScore { get; set; }
        public int PointsEarned { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string MatchDetails { get; set; } = string.Empty;
    }

    public class CreatePredictionDto
    {
        public int UserId { get; set; }
        public int MatchId { get; set; }
        public string PredictedOutcome { get; set; } = string.Empty; // HomeWin, AwayWin, Draw
        public int? PredictedHomeScore { get; set; }
        public int? PredictedAwayScore { get; set; }
    }
}