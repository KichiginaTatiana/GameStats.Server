using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using ApiContracts.Input;
using ApiContracts.Output;
using Kontur.GameStats.Server.Logic;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class ReportsTests : IntegrationTestBase
    {
        [Test]
        public void GetRecentMatchesTest()
        {
            var recentMatches = new List<RecentMatchContract>();

            Client.PutServer(PrimaryServer, Fixture.Build<ServerContract>()
                .With(x => x.gameModes, new[] {PrimaryGameMode})
                .Create());

            var scorebrd = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scorebrd.Add(Fixture.Create<ScoreboardItemContract>());

            var match =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scorebrd.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, match);
            recentMatches.Add(new RecentMatchContract()
            {
                serverEndpoint = PrimaryServer,
                timestamp = PrimaryTimestamp,
                results = match
            });

            for (var j = 0; j < 10; j++)
            {
                var scoreboard = new List<ScoreboardItemContract>();
                for (var i = 0; i < 2; i++)
                    scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

                var matchInfo =
                    Fixture.Build<MatchContract>()
                        .With(x => x.scoreboard, scoreboard.ToArray())
                        .With(x => x.gameMode, PrimaryGameMode)
                        .Create();
                Client.PutMatch(PrimaryServer, $"2017-01-11T{j:00}:17:00Z", matchInfo);
                recentMatches.Add(new RecentMatchContract()
                {
                    serverEndpoint = PrimaryServer,
                    timestamp = $"2017-01-11T{j:00}:17:00Z",
                    results = matchInfo
                });
            }
            recentMatches = recentMatches.OrderByDescending(x => x.timestamp).Take(5).ToList();

            TestHelper.WaitUntil(
                () => TestHelper.CompareArraysOfContracts(Client.GetRecentMatches().ToArray(), recentMatches.ToArray()),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetBestPlayersTest()
        {
            PutSomeData();
            var bestPlayers = new List<BestPlayerContract>()
            {
                // Name0 is immortal
                // Player{i} played less than 10 matches
                new BestPlayerContract {killToDeathRatio = 21.0, name = "Name1"},
                new BestPlayerContract {killToDeathRatio = 11.0/4, name = "Name2"},
            };

            TestHelper.WaitUntil(
                () => TestHelper.CompareArraysOfContracts(Client.GetBestPlayers().ToArray(), bestPlayers.ToArray()),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPopularServersTest()
        {
            PutSomeData();
            var popularServers = new List<PopularServerContract>()
            {
                new PopularServerContract
                {
                    averageMatchesPerDay = (double) 11/3,
                    endpoint = PrimaryServer,
                    name = "server111"
                },
                new PopularServerContract
                {
                    averageMatchesPerDay = 1.0,
                    endpoint = SecondaryServer,
                    name = "server1112"
                },
                new PopularServerContract
                {
                    averageMatchesPerDay = 0,
                    endpoint = EmptyServer,
                    name = "server11123"
                },
            };

            TestHelper.WaitUntil(
                () =>
                    TestHelper.CompareArraysOfContracts(Client.GetPopularServers().ToArray(), popularServers.ToArray()),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetRecentMatchesWithCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => Client.GetRecentMatches(1).Count() == 1, TaskPeriodInSec*2000);
        }

        [Test]
        public void GetBestPlayersWithCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => Client.GetBestPlayers(1).Count() == 1, TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPopularServersWithCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => Client.GetPopularServers(1).Count() == 1, TaskPeriodInSec*2000);
        }

        [Test]
        public void GetRecentMatchesWithNegativeCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetRecentMatches(-1).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetBestPlayersWithNegativeCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetBestPlayers(-1).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPopularServersWithNegativeCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetPopularServers(-1).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetRecentMatchesWithBigCountTest()
        {
            PutBigData();

            TestHelper.WaitUntil(
                () =>
                    Client.GetRecentMatches(CommonConstants.ReportMaxItemCount + 1).Count() ==
                    CommonConstants.ReportMaxItemCount, TaskPeriodInSec*3000);
        }

        [Test]
        public void GetBestPlayersWithBigCountTest()
        {
            PutBigData();

            TestHelper.WaitUntil(
                () =>
                    Client.GetBestPlayers(CommonConstants.ReportMaxItemCount + 1).Count() ==
                    CommonConstants.ReportMaxItemCount, TaskPeriodInSec*3000);
        }

        [Test]
        public void GetPopularServersWithBigCountTest()
        {
            PutBigData();

            TestHelper.WaitUntil(
                () =>
                    Client.GetPopularServers(CommonConstants.ReportMaxItemCount + 1).Count() ==
                    CommonConstants.ReportMaxItemCount, TaskPeriodInSec*3000);
        }

        private void PutBigData()
        {
            const string ip = "167.42.23.32";
            for (var j = 0; j < 51; j++)
            {
                Client.PutServer($"{ip}-{j}",
                    Fixture.Build<ServerContract>().With(x => x.gameModes, new[] { PrimaryGameMode }).Create());

                var scoreboard = new List<ScoreboardItemContract>();
                for (var i = 0; i < 51; i++)
                    scoreboard.Add(Fixture.Build<ScoreboardItemContract>().With(x => x.name, $"Name{i}").Create());

                var matchInfo =
                    Fixture.Build<MatchContract>()
                        .With(x => x.scoreboard, scoreboard.ToArray())
                        .With(x => x.gameMode, PrimaryGameMode)
                        .Create();
                Client.PutMatch($"{ip}-{j}", $"2017-01-11T11:17:{j:00}Z", matchInfo);
            }
        }

        [Test]
        public void GetRecentMatchesWithNullCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetRecentMatches(0).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetBestPlayersWithNullCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetBestPlayers(0).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPopularServersWithNullCountTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => !Client.GetPopularServers(0).Any(), TaskPeriodInSec*2000);
        }

        [Test]
        public void GetBestPlayersWithoutDeathsTest()
        {
            for (var j = 0; j < 10; j++)
            {
                Client.PutServer($"167.42.23.32-{j}",
                    Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

                var scoreboard = new List<ScoreboardItemContract>();
                for (var i = 0; i < 2; i++)
                    scoreboard.Add(Fixture.Build<ScoreboardItemContract>()
                        .With(x => x.deaths, 0)
                        .Create());

                var matchInfo =
                    Fixture.Build<MatchContract>()
                        .With(x => x.scoreboard, scoreboard.ToArray())
                        .With(x => x.gameMode, PrimaryGameMode)
                        .Create();
                Client.PutMatch($"167.42.23.32-{j}", $"2017-01-11T{j:00}:17:00Z", matchInfo);
            }

            TestHelper.WaitUntil(() => !Client.GetBestPlayers().Any(), TaskPeriodInSec*2000);
        }


        [Test]
        public void GetBestPlayersWithLessThanTenMatchesTest()
        {
            for (var j = 0; j < 9; j++)
            {
                Client.PutServer($"167.42.23.32-{j}",
                    Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

                var scoreboard = new List<ScoreboardItemContract>();
                for (var i = 0; i < 2; i++)
                    scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

                var matchInfo =
                    Fixture.Build<MatchContract>()
                        .With(x => x.scoreboard, scoreboard.ToArray())
                        .With(x => x.gameMode, PrimaryGameMode)
                        .Create();
                Client.PutMatch($"167.42.23.32-{j}", $"2017-01-11T{j:00}:17:00Z", matchInfo);
            }

            TestHelper.WaitUntil(() => !Client.GetBestPlayers().Any(), TaskPeriodInSec*2000);
        }

        [TestCase("/reports/recent-matches/")]
        [TestCase("/reports/best-players/")]
        [TestCase("/reports/popular-servers/")]
        public void GetReportWithBadCountTest(string url)
        {
            var getRequest = new RestRequest(url+Fixture.Create<string>(), Method.GET);
            var getResponse = Rc.Execute(getRequest);

            Assert.AreEqual(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Test]
        public void GetRecentMatchesWithNoData()
        {
            Thread.Sleep(2000);

            CollectionAssert.IsEmpty(Client.GetRecentMatches());
        }

        [Test]
        public void GetBestPlayersWithNoData()
        {
            Thread.Sleep(2000);

            CollectionAssert.IsEmpty(Client.GetBestPlayers());
        }

        [Test]
        public void GetPopularServersWithNoData()
        {
            Thread.Sleep(2000);

            CollectionAssert.IsEmpty(Client.GetPopularServers());
        }
    }
}