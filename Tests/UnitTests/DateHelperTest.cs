using Kontur.GameStats.Server.Logic;
using NUnit.Framework;

namespace Tests.UnitTests
{
    [TestFixture]
    public class DateHelperTest
    {
        private readonly DateHelper _dateHelper = new DateHelper();

        [TestCase("2017-03-03T12:00:00Z", "2017-03-03T12:00:01Z", ExpectedResult = 1)]

        [TestCase("2017-03-03T12:00:00Z", "2017-03-04T11:59:59Z", ExpectedResult = 2)]
        [TestCase("2017-03-03T12:00:00Z", "2017-03-04T12:00:00Z", ExpectedResult = 2)]
        [TestCase("2017-03-03T12:00:00Z", "2017-03-04T12:00:01Z", ExpectedResult = 2)]

        [TestCase("2017-03-03T00:00:00Z", "2017-03-04T23:59:59Z", ExpectedResult = 2)]
        [TestCase("2017-03-03T00:00:00Z", "2017-03-05T00:00:00Z", ExpectedResult = 3)]

        [TestCase("2017-03-03T12:00:00Z", "2018-03-03T12:00:00Z", ExpectedResult = 366)]
        public int GetTotalDaysTest(string since, string to)
        {
            return _dateHelper.GetTotalDays(since, to);
        }
    }
}