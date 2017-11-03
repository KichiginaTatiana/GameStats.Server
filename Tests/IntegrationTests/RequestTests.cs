using System.Net;
using ApiContracts.Input;
using NUnit.Framework;
using Ploeh.AutoFixture;
using RestSharp;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class RequestTests : IntegrationTestBase
    {
        [Test]
        public void ContentTypeTest()
        {
            var serverInputContract = Fixture.Create<ServerContract>();

            var putRequest1 = new RestRequest("servers/111.1.1.1-111/info", Method.PUT);
            putRequest1.AddJsonBody(serverInputContract);
            putRequest1.AddHeader("Content-Type", "application/json");
            var pp = Rc.Execute(putRequest1);
            TestHelper.WaitUntil(() => HttpStatusCode.OK == pp.StatusCode, TaskPeriodInSec * 1000);

            var getRequest1 = new RestRequest("servers/111.1.1.1-111/info", Method.GET);
            getRequest1.AddHeader("Content-Type", "application/json");
            var server1 = Rc.Execute(getRequest1).Content;

            var putRequest2 = new RestRequest("servers/111.1.1.1-1112/info", Method.PUT);
            putRequest2.AddJsonBody(serverInputContract);
            putRequest2.AddHeader("Content-Type", "application/xml");
            var p = Rc.Execute(putRequest2);
            TestHelper.WaitUntil(() => HttpStatusCode.OK == p.StatusCode, TaskPeriodInSec * 1000);

            var getRequest2 = new RestRequest("servers/111.1.1.1-1112/info", Method.GET);
            getRequest2.AddHeader("Content-Type", "application/xml");
            var server2 = Rc.Execute(getRequest2).Content;

            Assert.AreEqual(server1, server2);
        }

        [Test]
        public void CharsetTest()
        {
            var serverInputContract = Fixture.Create<ServerContract>();

            var putRequest1 = new RestRequest("servers/111.1.1.1-111/info", Method.PUT);
            putRequest1.AddJsonBody(serverInputContract);
            putRequest1.AddHeader("Content-Type", "application/json; charset=utf-8");
            TestHelper.WaitUntil(() => HttpStatusCode.OK == Rc.Execute(putRequest1).StatusCode, TaskPeriodInSec*1000);

            var getRequest1 = new RestRequest("servers/111.1.1.1-111/info", Method.GET);
            getRequest1.AddHeader("Content-Type", "application/json; charset=utf-8");
            var server1 = Rc.Execute(getRequest1).Content;

            var putRequest2 = new RestRequest("servers/111.1.1.1-1112/info", Method.PUT);
            putRequest2.AddJsonBody(serverInputContract);
            putRequest2.AddHeader("Content-Type", "application/json; charset=ISO-8859-1");
            var p = Rc.Execute(putRequest2);
            TestHelper.WaitUntil(() => HttpStatusCode.OK == p.StatusCode, TaskPeriodInSec*1000);

            var getRequest2 = new RestRequest("servers/111.1.1.1-1112/info", Method.GET);
            getRequest2.AddHeader("Content-Type", "application/json; charset=ISO-8859-1");
            var server2 = Rc.Execute(getRequest2).Content;

            Assert.AreEqual(server1, server2);
        }
    }
}
