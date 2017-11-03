using System;

namespace Kontur.GameStats.Server.Logic
{
    public class DateHelper
    {
        public bool CheckTimestamp(string timestamp)
        {
            DateTime result;
            return DateTime.TryParse(timestamp, out result);
        }

        public string GetDate(string timestamp)
        {
            return timestamp.Split(new[] { 'T' })[0];
        }

        public int GetTotalDays(string rawSince, string rawTo)
        {
            var since = DateTime.Parse(rawSince).ToUniversalTime();
            var to = DateTime.Parse(rawTo).ToUniversalTime();

            return (int) (to.Date - since.Date).TotalDays + 1;
        }
    }
}