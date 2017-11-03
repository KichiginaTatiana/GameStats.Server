using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Kontur.GameStats.Server.Infrastructure;
using Model;

namespace Kontur.GameStats.Server.Logic
{
    public class PlayerStatisticsCalculator
    {
        private readonly ConnectionProvider _connection;
        private readonly DateHelper _dateHelper;

        public PlayerStatisticsCalculator(ConnectionProvider connection, DateHelper dateHelper)
        {
            _connection = connection;
            _dateHelper = dateHelper;
        }

        public void Recalculate()
        {
            using (var entities = _connection.GetEntities())
            {
                var allMatches = entities.Matches.ToList();
                var matchesByWinner = allMatches
                    .GroupBy(m => m.Scoreboards.FirstOrDefault()?.Name)
                    .ToDictionary(g => g.Key, g => g.Count());

                var allScoreboards = entities.Scoreboards.ToList();
                var scoreboardsByPlayer = allScoreboards
                    .GroupBy(s => s.Name)
                    .ToDictionary(g => g.Key, g => g);

                foreach (var player in scoreboardsByPlayer.Keys)
                {
                    var playerScoreboards = scoreboardsByPlayer[player];
                    var playerMatches = playerScoreboards.Select(s => s.Match).ToList();

                    var playerStat = new PlayerStat
                    {
                        Name = player,
                        TotalMatchesPlayed = playerScoreboards.Count(),
                        TotalMatchesWon = matchesByWinner.ContainsKey(player) ? matchesByWinner[player] : 0
                    };

                    AddServerStat(playerStat, playerMatches);
                    AddGameModeStat(playerStat, playerMatches);
                    AddKillToDeathRatio(playerStat, playerScoreboards);
                    AddMatchStat(playerStat, playerMatches, allMatches);

                    playerStat.AverageScoreboardPercent = playerMatches.Average(m => GetScoreboardPercent(m, player));

                    entities.PlayerStats.AddOrUpdate(playerStat);
                }

                entities.SaveChanges();
            }
        }

        private static void AddServerStat(PlayerStat playerStat, List<Match> playerMatches)
        {
            var serverUsages = playerMatches
                .GroupBy(s => s.ServerEndpoint)
                .ToDictionary(g => g.Key, g => g.Count());

            playerStat.FavouriteServer = serverUsages
                .OrderByDescending(entry => entry.Value)
                .First()
                .Key;

            playerStat.UniqueServers = serverUsages.Count;
        }

        private static void AddGameModeStat(PlayerStat playerStat, List<Match> playerMatches)
        {
            playerStat.FavouriteGameMode = playerMatches
                .GroupBy(s => s.GameMode)
                .OrderByDescending(g => g.Count())
                .First()
                .Key;
        }

        private static void AddKillToDeathRatio(PlayerStat playerStat, IGrouping<string, Scoreboard> playerScoreboards)
        {
            var totalDeaths = playerScoreboards.Sum(s => s.Deaths);
            var totalKills = playerScoreboards.Sum(s => s.Kills);

            playerStat.KillToDeathRatio = totalDeaths != 0
                ? totalKills/(double) totalDeaths
                : 0.0;
        }

        private void AddMatchStat(PlayerStat playerStat, List<Match> playerMatches, List<Match> allMatches)
        {
            var firstMatchOfPlayer = playerMatches.Min(s => s.Timestamp);
            var lastMatchOfPlayer = playerMatches.Max(s => s.Timestamp);
            var lastMatchEver = allMatches.Max(s => s.Timestamp);

            playerStat.LastMatchPlayed = lastMatchOfPlayer;
            playerStat.AverageMatchesPerDay = playerStat.TotalMatchesPlayed/
                                              (double) _dateHelper.GetTotalDays(firstMatchOfPlayer, lastMatchEver);

            playerStat.MaximumMatchesPerDay = playerMatches
                .GroupBy(s => _dateHelper.GetDate(s.Timestamp))
                .Max(g => g.Count());
        }

        private double GetScoreboardPercent(Match match, string player)
        {
            var scoreboards = match.Scoreboards.OrderBy(s => s.Id);
            var enemiesCount = scoreboards.Count() - 1;

            var playersAboveCurrent = scoreboards.Select(s => s.Name).ToList().IndexOf(player);
            var playersBelowCurrent = enemiesCount - playersAboveCurrent;
            return scoreboards.Count() == 1
                ? 100.0
                : 100.0*playersBelowCurrent/enemiesCount;
        }
    }
}