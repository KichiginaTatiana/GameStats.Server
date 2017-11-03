namespace ApiContracts.Output
{
    public class PlayerStatContract
    {
        public long totalMatchesPlayed { get; set; }
        public long totalMatchesWon { get; set; }
        public string favouriteServer { get; set; }
        public long uniqueServers { get; set; }
        public string favouriteGameMode { get; set; }
        public double averageScoreboardPercent { get; set; }
        public long maximumMatchesPerDay { get; set; }
        public double averageMatchesPerDay { get; set; }
        public string lastMatchPlayed { get; set; }
        public double killToDeathRatio { get; set; }
    }
}
