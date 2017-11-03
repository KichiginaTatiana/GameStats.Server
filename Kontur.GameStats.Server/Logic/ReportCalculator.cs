using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Kontur.GameStats.Server.Infrastructure;
using Model;

namespace Kontur.GameStats.Server.Logic
{
    public class ReportCalculator
    {
        private readonly ConnectionProvider _connection;

        public ReportCalculator(ConnectionProvider connection)
        {
            _connection = connection;
        }

        public void BuildRecentMatchesReport()
        {
            using (var entities = _connection.GetEntities())
            {
                var recentMatches = entities.Matches
                    .ToList().OrderByDescending(x => x.Timestamp)
                    .Take(CommonConstants.ReportMaxItemCount);

                Rewrite(entities.RecentMatches, recentMatches.Select(m => new RecentMatch
                {
                    ServerEndpoint = m.ServerEndpoint,
                    Timestamp = m.Timestamp
                }));

                entities.SaveChanges();
            }
        }

        public void BuildBestPlayersReport()
        {
            using (var entities = _connection.GetEntities())
            {
                var immortalPlayers = entities.Scoreboards
                    .GroupBy(s => s.Name)
                    .ToDictionary(g => g.Key, g => g.Sum(s => s.Deaths))
                    .Where(p => p.Value == 0)
                    .Select(p => p.Key)
                    .ToList();

                var playerStats = entities.PlayerStats.ToList()
                    .Where(p => p.TotalMatchesPlayed >= 10 && !immortalPlayers.Contains(p.Name))
                    .OrderByDescending(x => x.KillToDeathRatio)
                    .Take(CommonConstants.ReportMaxItemCount);

                Rewrite(entities.BestPlayers, playerStats.Select(p => new BestPlayer
                {
                    KillToDeathRatio = p.KillToDeathRatio,
                    Name = p.Name
                }));

                entities.SaveChanges();
            }
        }

        public void BuildPopularServersReport()
        {
            using (var entities = _connection.GetEntities())
            {
                var servers = entities.ServerStats
                    .ToList().OrderByDescending(x => x.AverageMatchesPerDay)
                    .Take(CommonConstants.ReportMaxItemCount);

                Rewrite(entities.PopularServers, servers.Select(s => new PopularServer
                {
                    AverageMatchesPerDay = s.AverageMatchesPerDay,
                    Endpoint = s.Endpoint,
                    Name = s.Name
                }));

                entities.SaveChanges();
            }
        }

        private static void Rewrite<T>(IDbSet<T> set, IEnumerable<T> entities) where T : class
        {
            foreach (var entity in set)
                set.Remove(entity);

            foreach (var entity in entities)
                set.Add(entity);
        }
    }
}