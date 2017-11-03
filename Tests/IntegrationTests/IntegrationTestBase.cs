using System;
using System.Collections.Generic;
using System.IO;
using ApiClient;
using ApiContracts.Input;
using Kontur.GameStats.Server.Infrastructure;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;

namespace Tests.IntegrationTests
{
    public class IntegrationTestBase
    {
        private const string Url = "http://localhost:8080";
        protected const int TaskPeriodInSec = 5;
        protected readonly Client Client = new Client(Url);
        protected readonly Fixture Fixture = new Fixture();
        protected readonly RestClient Rc = new RestClient(Url);

        protected const string PrimaryServer = "167.42.23.32-111";
        protected const string SecondaryServer = "167.42.23.32-1112";
        protected const string EmptyServer = "167.42.23.32-11123";

        protected const string PrimaryGameMode = "DM";
        protected const string SecondaryGameMode = "TDM";

        protected const string PrimaryTimestamp = "2017-01-12T11:17:00Z";
        protected const string SecondaryTimestamp = "2017-01-12T12:17:00Z";

        [OneTimeSetUp]
        public void SetUpOnce()
        {
            TestHelper.KillProcess();
            TestHelper.StartProcess($"--taskPeriod {TaskPeriodInSec}");
        }

        [SetUp]
        public void SetUp()
        {
            ClearDb();
        }

        [OneTimeTearDown]
        public void TearDownOnce()
        {
            TestHelper.KillProcess();
        }

        private static void ClearDb()
        {
            var dbDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..",
                "Kontur.GameStats.Server", "bin", "Release");
            var entities = new ConnectionProvider().GetEntities(dbDirectory);

            using (entities)
            {
                entities.Database.ExecuteSqlCommand("delete from Scoreboards");
                entities.Database.ExecuteSqlCommand("delete from GameModes");
                entities.Database.ExecuteSqlCommand("delete from Matches");
                entities.Database.ExecuteSqlCommand("delete from Servers");

                entities.Database.ExecuteSqlCommand("delete from BestPlayers");
                entities.Database.ExecuteSqlCommand("delete from PopularServers");
                entities.Database.ExecuteSqlCommand("delete from RecentMatches");
                entities.Database.ExecuteSqlCommand("delete from PlayerStats");
                entities.Database.ExecuteSqlCommand("delete from TopFiveMaps");
                entities.Database.ExecuteSqlCommand("delete from TopFiveGameModes");
                entities.Database.ExecuteSqlCommand("delete from ServerStats");
                entities.SaveChanges();
            }
        }

        protected void PutSomeData()
        {
            PutServers();

            var scorebrd = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scorebrd.Add(
                    Fixture.Build<ScoreboardItemContract>()
                        .With(x => x.name, $"Name{i}")
                        .With(x => x.deaths, 0 + i*i)
                        .With(x => x.kills, 31 - 10*i)
                        .Create());

            var match =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scorebrd.ToArray())
                    .With(x => x.map, "DM-HelloWorld0")
                    .With(x => x.gameMode, SecondaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, match);

            var scorebrd1 = new List<ScoreboardItemContract>();
            for (var i = 0; i < 2; i++)
                scorebrd1.Add(
                    Fixture.Build<ScoreboardItemContract>()
                        .With(x => x.name, $"Player{i}")
                        .With(x => x.deaths, 2 + i * i)
                        .With(x => x.kills, 31 - 10 * i)
                        .Create());

            var match1 =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scorebrd1.ToArray())
                    .With(x => x.map, "DM-HelloWorld0")
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(SecondaryServer, "2017-01-13T11:17:00Z", match1);

            for (var j = 0; j < 10; j++)
            {
                var scoreboard = new List<ScoreboardItemContract>();
                for (var i = 0; i < 3; i++)
                    scoreboard.Add(
                        Fixture.Build<ScoreboardItemContract>()
                            .With(x => x.name, $"Name{i}")
                            .With(x => x.deaths, 0 + i*i)
                            .With(x => x.kills, 31 - 10*i)
                            .Create());

                var matchInfo =
                    Fixture.Build<MatchContract>()
                        .With(x => x.scoreboard, scoreboard.ToArray())
                        .With(x => x.map, $"DM-HelloWorld{j%2}")
                        .With(x => x.gameMode, PrimaryGameMode)
                        .Create();
                Client.PutMatch(PrimaryServer, $"2017-01-11T02:{j:00}:00Z", matchInfo);
            }
        }

        private void PutServers()
        {
            Client.PutServer(PrimaryServer,
                new ServerContract()
                {
                    name = "server111",
                    gameModes = new[] {PrimaryGameMode, SecondaryGameMode}
                });

            Client.PutServer(SecondaryServer,
                new ServerContract()
                {
                    name = "server1112",
                    gameModes = new[] {PrimaryGameMode}
                });

            Client.PutServer(EmptyServer,
                new ServerContract()
                {
                    name = "server11123",
                    gameModes = new[] {PrimaryGameMode}
                });
        }
    }
}
