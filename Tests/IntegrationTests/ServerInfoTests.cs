using System.Linq;
using System.Net;
using System.Threading;
using ApiContracts.Input;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class ServerInfoTests : IntegrationTestBase
    {
        [Test]
        public void PutNoDataTest()
        {
            Assert.AreEqual(HttpStatusCode.BadRequest, Client.PutServer(PrimaryServer, null));
        }

        [Test]
        public void PutThenGetTest()
        {
            var serverInfo = Fixture.Create<ServerContract>();

            Assert.AreEqual(HttpStatusCode.OK, Client.PutServer(PrimaryServer, serverInfo));

            Assert.True(TestHelper.CompareContracts(serverInfo, Client.GetServer(PrimaryServer)));
        }

        [Test]
        public void PutThenUpdateTest()
        {
            var serverInfo = Fixture.Create<ServerContract>();
            Assert.AreEqual(HttpStatusCode.OK, Client.PutServer(PrimaryServer, serverInfo));

            serverInfo = Fixture.Create<ServerContract>();
            Assert.AreEqual(HttpStatusCode.OK, Client.PutServer(PrimaryServer, serverInfo));

            Assert.True(TestHelper.CompareContracts(serverInfo, Client.GetServer(PrimaryServer)));
        }

        [Test]
        public void GetServersTest()
        {
            Thread.Sleep(2000);

            var server1 = Fixture.Build<ServerInfoContract>().With(x => x.endpoint, PrimaryServer).Create();
            var server2 = Fixture.Build<ServerInfoContract>().With(x => x.endpoint, SecondaryServer).Create();

            Client.PutServer(server1.endpoint, server1.info);
            Client.PutServer(server2.endpoint, server2.info);

            Assert.True(TestHelper.CompareArraysOfContracts(new ServerInfoContract[] {server1, server2},
                Client.GetServers().ToArray()));
        }

        [Test]
        public void GetNonexistentServerTest()
        {
            Assert.Null(Client.GetServer(PrimaryServer));
        }

        [Test]
        public void GetNullServersTest()
        {
            CollectionAssert.IsEmpty(Client.GetServers());
        }
    }
}
