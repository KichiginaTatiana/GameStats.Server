namespace Kontur.GameStats.Server.Logic
{
    public class StatisticsCalculator
    {
        private readonly ServerStatisticsCalculator _serverStatisticsCalculator;
        private readonly PlayerStatisticsCalculator _playerStatisticsCalculator;
        private readonly ReportCalculator _reportCalculator;

        public StatisticsCalculator(ServerStatisticsCalculator serverStatisticsCalculator, PlayerStatisticsCalculator playerStatisticsCalculator, ReportCalculator reportCalculator)
        {
            _serverStatisticsCalculator = serverStatisticsCalculator;
            _playerStatisticsCalculator = playerStatisticsCalculator;
            _reportCalculator = reportCalculator;
        }

        public void RecalculateAll()
        {
            _serverStatisticsCalculator.Recalculate();
            _playerStatisticsCalculator.Recalculate();
            _reportCalculator.BuildRecentMatchesReport();
            _reportCalculator.BuildBestPlayersReport();
            _reportCalculator.BuildPopularServersReport();
        }
    }
}
