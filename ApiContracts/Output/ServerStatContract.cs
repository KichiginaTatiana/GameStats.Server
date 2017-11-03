namespace ApiContracts.Output
{
    public class ServerStatContract
    {
        public long totalMatchesPlayed { get; set; }
        public long maximumMatchesPerDay { get; set; }
        public double averageMatchesPerDay { get; set; }
        public int maximumPopulation { get; set; }
        public double averagePopulation { get; set; }
        public string[] top5GameModes { get; set; }
        public string[] top5Maps { get; set; }
    }
}
