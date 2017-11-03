using System.Collections.Generic;
using ApiContracts.Input;
using ApiContracts.Output;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class StatTests : IntegrationTestBase
    {
        [Test]
        public void GetServerStatsTest()
        {
            PutSomeData();
            var serverStats = new ServerStatContract()
            {
                averageMatchesPerDay = (double) 11/3,
                averagePopulation = 3,
                maximumMatchesPerDay = 10,
                maximumPopulation = 3,
                totalMatchesPlayed = 11,
                top5GameModes = new[] {PrimaryGameMode, SecondaryGameMode},
                top5Maps = new[] {"DM-HelloWorld0", "DM-HelloWorld1"}
            };

            TestHelper.WaitUntil(
                () => TestHelper.CompareContracts(Client.GetServerStat(PrimaryServer), serverStats),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetServerWithoutMatchesStatsTest()
        {
            PutSomeData();
            var serverStats = new ServerStatContract()
            {
                averageMatchesPerDay = 0,
                averagePopulation = 0,
                maximumMatchesPerDay = 0,
                maximumPopulation = 0,
                totalMatchesPlayed = 0,
                top5GameModes = new string[0],
                top5Maps = new string[0]
            };

            TestHelper.WaitUntil(
                () => TestHelper.CompareContracts(Client.GetServerStat(EmptyServer), serverStats),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetNonexistentServerStatsTest()
        {
            Assert.Null(Client.GetServerStat(PrimaryServer));
        }

        [Test]
        public void GetPlayerStatsTest()
        {
            PutSomeData();
            var player = new PlayerStatContract()
            {
                averageMatchesPerDay = (double) 11/3,
                maximumMatchesPerDay = 10,
                totalMatchesPlayed = 11,
                averageScoreboardPercent = 50,
                favouriteGameMode = PrimaryGameMode,
                favouriteServer = PrimaryServer,
                killToDeathRatio = 21.0,
                uniqueServers = 1,
                lastMatchPlayed = PrimaryTimestamp,
                totalMatchesWon = 0
            };

            TestHelper.WaitUntil(() => TestHelper.CompareContracts(Client.GetPlayerStat("Name1"), player),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetNonexistentPlayerStatsTest()
        {
            Assert.Null(Client.GetPlayerStat(Fixture.Create<string>()));
        }

        [Test]
        public void UpdatingServerStatTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => Client.GetServerStat(PrimaryServer)?.averageMatchesPerDay == (double)11 / 3,
                TaskPeriodInSec * 2000);

            var scorebrd = new List<ScoreboardItemContract>();
            for (var i = 0; i < 2; i++)
                scorebrd.Add(
                    Fixture.Create<ScoreboardItemContract>());

            var match =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scorebrd.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, SecondaryTimestamp, match);

            TestHelper.WaitUntil(() => Client.GetServerStat(PrimaryServer)?.averageMatchesPerDay == 4,
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPlayerStatsIgnoreCaseTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(
                () =>
                    TestHelper.CompareContracts(Client.GetPlayerStat("Name0"), Client.GetPlayerStat("Name0".ToLower())),
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetSingularPlayerInMatchStatsTest()
        {
            Client.PutServer(PrimaryServer, new ServerContract()
            {
                name = "server111",
                gameModes = new[] {PrimaryGameMode, SecondaryGameMode}
            });
 
             var scoreboard = new List<ScoreboardItemContract>
            {
                Fixture.Build<ScoreboardItemContract>().With(x => x.name, "SingularPlayer").Create()
            };

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo);

            TestHelper.WaitUntil(() => Client.GetPlayerStat("SingularPlayer")?.averageScoreboardPercent == 100,
                TaskPeriodInSec*2000);
        }

        [Test]
        public void UpdatingPlayerStatTest()
        {
            PutSomeData();

            TestHelper.WaitUntil(() => Client.GetPlayerStat("Name0")?.averageMatchesPerDay == (double) 11/3,
                TaskPeriodInSec*2000);

            var scorebrd = new List<ScoreboardItemContract>();
            for (var i = 0; i < 2; i++)
                scorebrd.Add(
                    Fixture.Build<ScoreboardItemContract>()
                        .With(x => x.name, $"Name{i}")
                        .Create());

            var match =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scorebrd.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, SecondaryTimestamp, match);

            TestHelper.WaitUntil(() => Client.GetPlayerStat("Name0")?.averageMatchesPerDay == 4,
                TaskPeriodInSec*2000);
        }

        [Test]
        public void GetPlayerStatWithEncodedNameTest()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(
                    Fixture.Build<ScoreboardItemContract>()
                        .With(x => x.name, $"Name {i}")
                        .With(x => x.deaths, 1)
                        .With(x => x.kills, 11)
                        .Create());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo);

            var player = new PlayerStatContract()
            {
                averageMatchesPerDay = 1,
                maximumMatchesPerDay = 1,
                totalMatchesPlayed = 1,
                averageScoreboardPercent = 100,
                favouriteGameMode = PrimaryGameMode,
                favouriteServer = PrimaryServer,
                killToDeathRatio = 11,
                uniqueServers = 1,
                lastMatchPlayed = PrimaryTimestamp,
                totalMatchesWon = 1
            };

            TestHelper.WaitUntil(() => TestHelper.CompareContracts(player, Client.GetPlayerStat("Name+0")),
                TaskPeriodInSec*2000);
        }
    }
}
