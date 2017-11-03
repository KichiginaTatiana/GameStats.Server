namespace ApiContracts.Input
{
    public class MatchContract
    {
        public string gameMode { get; set; }
        public string map { get; set; }
        public long fragLimit { get; set; }
        public long timeLimit { get; set; }
        public double timeElapsed { get; set; }
        public ScoreboardItemContract[] scoreboard { get; set; }
    }

    public class ScoreboardItemContract
    {
        public string name { get; set; }
        public long frags { get; set; }
        public long kills { get; set; }
        public long deaths { get; set; }
    }
}
