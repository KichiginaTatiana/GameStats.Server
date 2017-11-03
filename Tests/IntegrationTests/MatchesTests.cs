using System.Collections.Generic;
using System.Net;
using ApiContracts.Input;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class MatchesTest : IntegrationTestBase
    {
        [Test]
        public void PutNoDataTest()
        {
            Client.PutServer(PrimaryServer, Fixture.Create<ServerContract>());

            Assert.AreEqual(HttpStatusCode.BadRequest, Client.PutMatch(PrimaryServer, PrimaryTimestamp, null));
        }

        [Test]
        public void PutThenGetTest()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo);

            var actual = Client.GetMatch(PrimaryServer, PrimaryTimestamp);
            Assert.True(TestHelper.CompareContracts(matchInfo, actual));
        }

        [Test]
        public void GetNonexistentMatchTest()
        {
            Client.PutServer(PrimaryServer, Fixture.Create<ServerContract>());

            Assert.Null(Client.GetMatch(PrimaryServer, PrimaryTimestamp));
        }

        [Test]
        public void PutMatchWithBadTimestampTest()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(PrimaryServer, Fixture.Create<string>(), matchInfo));
        }

        [Test]
        public void PutMatchOnNonexistentServerTest()
        {
            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .Create();

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(Fixture.Create<string>(), PrimaryTimestamp, matchInfo));
        }

        [Test]
        public void PutExistedMatchTest()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] {PrimaryGameMode}).Create());

            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();
            Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo);

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo));
        }

        [Test]
        public void GetMatchWithBadTimestampTest()
        {
            Client.PutServer(PrimaryServer, Fixture.Create<ServerContract>());

            Assert.Null(Client.GetMatch(PrimaryServer, Fixture.Create<string>()));
        }

        [Test]
        public void GetMatchWithBadServerEndpointTest()
        {
            Assert.Null(Client.GetMatch(Fixture.Create<string>(), PrimaryTimestamp));
        }

        [Test]
        public void PutMatchWithBadServerEndpointTest()
        {
            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(Fixture.Create<string>(), PrimaryTimestamp, matchInfo));
        }

        [Test]
        public void PutMatchWithoutScoreboard()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] { PrimaryGameMode }).Create());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, new ScoreboardItemContract[0])
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(PrimaryServer, PrimaryTimestamp, matchInfo));
        }

        [Test]
        public void PutMatchWithIncorrectTimestampTest()
        {
            Client.PutServer(PrimaryServer,
                Fixture.Build<ServerContract>().With(x => x.gameModes, new[] { PrimaryGameMode }).Create());

            var scoreboard = new List<ScoreboardItemContract>();
            for (var i = 0; i < 3; i++)
                scoreboard.Add(Fixture.Create<ScoreboardItemContract>());

            var matchInfo =
                Fixture.Build<MatchContract>()
                    .With(x => x.scoreboard, scoreboard.ToArray())
                    .With(x => x.gameMode, PrimaryGameMode)
                    .Create();

            Assert.AreEqual(HttpStatusCode.BadRequest,
                Client.PutMatch(PrimaryServer, "2017-41-41T41:77:700Z", matchInfo));
        }
    }
}