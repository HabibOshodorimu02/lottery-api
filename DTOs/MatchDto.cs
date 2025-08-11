namespace LotteryPredictionApi.DTOs
{
    public class MatchDto
    {
        public int Id { get; set; }
        public string HomeTeam { get; set; } = string.Empty;
        public string AwayTeam { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public bool IsCompleted { get; set; }
        public string? Result { get; set; }
    }
}