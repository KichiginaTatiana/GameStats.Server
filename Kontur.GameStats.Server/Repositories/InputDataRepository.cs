using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ApiContracts.Input;
using Kontur.GameStats.Server.Infrastructure;
using Model;

namespace Kontur.GameStats.Server.Repositories
{
    public class InputDataRepository
    {
        private readonly ConnectionProvider _connectionProvider;

        public InputDataRepository(ConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public void PutServerInfo(string endpoint, ServerContract contract)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var server = new Model.Server
                {
                    Endpoint = endpoint,
                    Name = contract.name,
                };
                var gameModes = contract.gameModes.Select(m => new GameMode
                {
                    Name = m,
                    ServerEndpoint = endpoint
                }).ToList();

                var existedServer = entities.Servers.Find(endpoint);
                if (existedServer != null)
                    entities.GameModes.RemoveRange(existedServer.GameModes);

                entities.Servers.AddOrUpdate(server);
                entities.GameModes.AddRange(gameModes);

                entities.SaveChanges();
            }
        }

        public bool PutMatchInfo(string endpoint, string timestamp, MatchContract contract)
        {
            if (contract.scoreboard.Length == 0)
                return false;

            using (var entities = _connectionProvider.GetEntities())
            {
                var server = entities.Servers.Find(endpoint);
                if (server == null ||
                    server.GameModes.All(m => m.Name != contract.gameMode) ||
                    entities.Matches.Any(m => m.Timestamp == timestamp && m.ServerEndpoint == endpoint))
                    return false;

                entities.Matches.Add(new Match
                {
                    FragLimit = contract.fragLimit,
                    GameMode = contract.gameMode,
                    Map = contract.map,
                    ServerEndpoint = endpoint,
                    Timestamp = timestamp,
                    TimeElapsed = contract.timeElapsed,
                    TimeLimit = contract.timeLimit,
                });

                foreach (var s in contract.scoreboard)
                {
                    entities.Scoreboards.Add(new Scoreboard
                    {
                        Deaths = s.deaths,
                        Frags = s.frags,
                        Kills = s.kills,
                        Name = s.name,
                    });
                }

                entities.SaveChanges();
            }
            return true;
        }

        public ServerContract GetServerInfo(string endpoint)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var server = entities.Servers.Find(endpoint);
                if (server == null)
                    return null;

                return new ServerContract()
                {
                    name = server.Name,
                    gameModes = server.GameModes.Select(m => m.Name).ToArray()
                };
            }
        }

        public List<ServerInfoContract> GetServerInfos()
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var servers = entities.Servers.ToList();
                var gameModes = entities.GameModes.ToList();

                return servers.Select(s => new ServerInfoContract
                {
                    endpoint = s.Endpoint,
                    info = new ServerContract
                    {
                        name = s.Name,
                        gameModes = gameModes.Where(m => m.Server == s).Select(m => m.Name).ToArray()
                    }
                }).ToList();
            }
        }

        public MatchContract GetMatchesInfo(string endpoint, string timestamp)
        {
            using (var entities = _connectionProvider.GetEntities())
            {
                var match =
                    entities.Matches.FirstOrDefault(
                        x => x.ServerEndpoint == endpoint && x.Timestamp == timestamp);
                if (match == null)
                    return null;

                return new MatchContract()
                {
                    fragLimit = match.FragLimit,
                    gameMode = match.GameMode,
                    map = match.Map,
                    timeElapsed = match.TimeElapsed,
                    timeLimit = match.TimeLimit,
                    scoreboard = match.Scoreboards.Select(scoreboardItem => new ScoreboardItemContract
                    {
                        deaths = scoreboardItem.Deaths,
                        frags = scoreboardItem.Frags,
                        name = scoreboardItem.Name,
                        kills = scoreboardItem.Kills
                    }).ToArray()
                };
            }
        }
    }
}
