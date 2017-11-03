using System.Threading;
using NUnit.Framework;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class StartHostTests
    {
        [TestCase("http://+:8080/")]
        [TestCase("https://www.google.ru/")]
        [TestCase("111.111.1.1:8080")]
        [TestCase("[3ffe:ffff::6ECB:0101]:8080")]
        [TestCase("http://*:8080/")]
        [TestCase("http://localhost:8080/fr/")]
        public void PrefixTest(string prefix)
        {
            TestHelper.StartProcess($"--prefix {prefix}");

            Thread.Sleep(5000);

            Assert.True(TestHelper.KillProcess());
        }
    }
}
