using System.Collections.Generic;
using System.Linq;
using ApiContracts.Input;
using ApiContracts.Output;
using Kontur.GameStats.Server.Infrastructure;

namespace Kontur.GameStats.Server.Repositories
{
    public class OutputDataRepository
    {
        private readonly ConnectionProvider _connectionProvider;

        public OutputDataRepository(ConnectionProvider connectionProvider)
        {
            this._connectionProvider = connectionProvider;
        }

        public ServerStatContract GetServerStat(string endpoint)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var server = entities.ServerStats.Find(endpoint);

                return server == null
                    ? null
                    : new ServerStatContract
                    {
                        averageMatchesPerDay = server.AverageMatchesPerDay,
                        averagePopulation = server.AveragePopulation,
                        maximumMatchesPerDay = server.MaximumMatchesPerDay,
                        maximumPopulation = server.MaximumPopulation,
                        totalMatchesPlayed = server.TotalMatchesPlayed,
                        top5GameModes = server.TopFiveGameModes.Select(m => m.GameMode).ToArray(),
                        top5Maps = server.TopFiveMaps.Select(m => m.Map).ToArray()
                    };
            }
        }

        public PlayerStatContract GetPlayerStat(string name)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var player = entities.PlayerStats.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());

                return player == null
                    ? null
                    : new PlayerStatContract
                    {
                        totalMatchesPlayed = player.TotalMatchesPlayed,
                        averageMatchesPerDay = player.AverageMatchesPerDay,
                        averageScoreboardPercent = player.AverageScoreboardPercent,
                        favouriteGameMode = player.FavouriteGameMode,
                        favouriteServer = player.FavouriteServer,
                        killToDeathRatio = player.KillToDeathRatio,
                        lastMatchPlayed = player.LastMatchPlayed,
                        maximumMatchesPerDay = player.MaximumMatchesPerDay,
                        totalMatchesWon = player.TotalMatchesWon,
                        uniqueServers = player.UniqueServers
                    };
            }
        }

        public List<RecentMatchContract> GetRecentMatches(int count)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var matches = entities.RecentMatches.OrderByDescending(x => x.Timestamp).Take(count);
                var result = new List<RecentMatchContract>();

                foreach (var m in matches)
                {
                    var match =
                        entities.Matches.First(
                            x => x.Timestamp == m.Timestamp && x.ServerEndpoint == m.ServerEndpoint);

                    result.Add(new RecentMatchContract()
                    {
                        serverEndpoint = match.ServerEndpoint,
                        timestamp = match.Timestamp,
                        results = new MatchContract()
                        {
                            fragLimit = match.FragLimit,
                            gameMode = match.GameMode,
                            map = match.Map,
                            timeElapsed = match.TimeElapsed,
                            timeLimit = match.TimeLimit,
                            scoreboard = match.Scoreboards.Select(scoreboardItem => new ScoreboardItemContract()
                            {
                                deaths = scoreboardItem.Deaths,
                                frags = scoreboardItem.Frags,
                                kills = scoreboardItem.Kills,
                                name = scoreboardItem.Name
                            }).ToArray()
                        }
                    });
                }
                return result;
            }
        }

        public List<BestPlayerContract> GetBestPlayers(int count)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var players = entities.BestPlayers.OrderByDescending(x => x.KillToDeathRatio).Take(count).ToList();
                return players.Select(player => new BestPlayerContract()
                {
                    name = player.Name,
                    killToDeathRatio = player.KillToDeathRatio
                }).ToList();
            }
        }

        public List<PopularServerContract> GetPopularServers(int count)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var servers = entities.PopularServers.Take(count).ToList();
                return servers.Select(server => new PopularServerContract()
                {
                    name = server.Name,
                    endpoint = server.Endpoint,
                    averageMatchesPerDay = server.AverageMatchesPerDay
                }).ToList();
            }
        }
    }
}
