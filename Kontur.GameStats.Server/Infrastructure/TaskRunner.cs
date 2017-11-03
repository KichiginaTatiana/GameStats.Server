using System;
using System.Diagnostics;
using System.Threading.Tasks;
using log4net;

namespace Kontur.GameStats.Server.Infrastructure
{
    public class TaskRunner
    {
        private static readonly ILog Log = LogManager.GetLogger(nameof(Program));

        public async void Run(Action action, int taskPeriodInSec)
        {
            while (true)
            {
                try
                {
                    var lastTaskDuration = MeasureTime(action);

                    var delay = TimeSpan.FromSeconds(taskPeriodInSec) - lastTaskDuration - lastTaskDuration;
                    if (delay.Ticks > 0)
                        await Task.Delay(delay);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private static TimeSpan MeasureTime(Action action)
        {
            Log.Info("Start task");
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            Log.Info("Finish task");
            return stopwatch.Elapsed;
        }
    }
}