using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Kontur.GameStats.Server.Infrastructure;
using Model;

namespace Kontur.GameStats.Server.Logic
{
    public class ServerStatisticsCalculator
    {
        private readonly ConnectionProvider _connection;
        private readonly DateHelper _dateHelper;

        public ServerStatisticsCalculator(ConnectionProvider connection, DateHelper dateHelper)
        {
            _connection = connection;
            _dateHelper = dateHelper;
        }

        public void Recalculate()
        {
            using (var entities = _connection.GetEntities())
            {
                var servers = entities.Servers.Include("Matches").Include("GameModes").ToList();
                var allMatches = entities.Matches.Include("Scoreboards").ToList();

                foreach (var server in servers)
                {
                    var matches = server.Matches.ToList();

                    var serverStat = new ServerStat
                    {
                        Endpoint = server.Endpoint,
                        Name = server.Name,
                        TotalMatchesPlayed = matches.Count(),
                    };

                    AddPopulationStat(serverStat, matches);
                    AddMatchesStat(serverStat, matches, allMatches);

                    var top5GameModes = GetTop5(matches, m => m.GameMode);
                    var top5Maps = GetTop5(matches, m => m.Map);

                    Rewrite(entities, serverStat, top5GameModes, top5Maps);
                }

                entities.SaveChanges();
            }
        }

        private static void AddPopulationStat(ServerStat serverStat, IEnumerable<Match> matches)
        {
            var totalPopulation = 0;

            foreach (var match in matches)
            {
                var matchPopulation = match.Scoreboards.Count;

                totalPopulation += matchPopulation;
                if (matchPopulation > serverStat.MaximumPopulation)
                    serverStat.MaximumPopulation = matchPopulation;
            }

            serverStat.AveragePopulation = matches.Any()
                ? totalPopulation / (double) serverStat.TotalMatchesPlayed
                : 0;
        }

        private void AddMatchesStat(ServerStat serverStat, List<Match> serverMatches, List<Match> allMatches)
        {
            if (!serverMatches.Any())
                return;

            serverStat.MaximumMatchesPerDay = serverMatches
                .GroupBy(m => _dateHelper.GetDate(m.Timestamp))
                .Max(g => g.Count());

            var firstDateOnServer = serverMatches.Min(x => x.Timestamp);
            var lastDateEver = allMatches.Max(x => x.Timestamp);
            var totalDays = _dateHelper.GetTotalDays(firstDateOnServer, lastDateEver);

            serverStat.AverageMatchesPerDay = serverStat.TotalMatchesPlayed/(double) totalDays;
        }

        private static List<string> GetTop5(List<Match> matches, Func<Match, string> getter)
        {
            return matches.GroupBy(getter)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => g.Key)
                .ToList();
        }

        private static void Rewrite(Entities entities, ServerStat serverStat, List<string> top5GameModes, List<string> top5Maps)
        {
            var existedServerStats = entities.ServerStats.Find(serverStat.Endpoint);
            if (existedServerStats != null)
            {
                entities.TopFiveGameModes.RemoveRange(existedServerStats.TopFiveGameModes);
                entities.TopFiveMaps.RemoveRange(existedServerStats.TopFiveMaps);
            }

            entities.ServerStats.AddOrUpdate(serverStat);

            foreach (var g in top5GameModes)
                entities.TopFiveGameModes.Add(new TopFiveGameMode
                {
                    ServerEndpoint = serverStat.Endpoint,
                    GameMode = g
                });

            foreach (var m in top5Maps)
                entities.TopFiveMaps.Add(new TopFiveMap
                {
                    ServerEndpoint = serverStat.Endpoint,
                    Map = m
                });
        }
    }
}