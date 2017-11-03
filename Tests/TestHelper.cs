using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Tests
{
    public static class TestHelper
    {
        public static void StartProcess(string arguments = "")
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "Kontur.GameStats.Server",
                "bin", "Release", "Kontur.GameStats.Server.exe");
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = path,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Arguments = arguments
                }
            };

            proc.Start();
        }

        public static bool KillProcess()
        {
            var processes = Process.GetProcessesByName("Kontur.GameStats.Server");

            foreach (var process in processes)
            {
                process.Kill();
                process.WaitForExit();
            }

            return processes.Any();
        }

        public static void WaitUntil(Func<bool> predicate, int timeoutInMs)
        {
            var sw = Stopwatch.StartNew();
            while (sw.Elapsed.TotalMilliseconds < timeoutInMs)
            {
                if (predicate())
                    return;
                Thread.Sleep(1000);
            }
            throw new Exception("Timeout exceeded");
        }

        public static bool CompareContracts<T>(T contract1, T contract2)
        {
            return JsonConvert.SerializeObject(contract1) == JsonConvert.SerializeObject(contract2);
        }

        public static bool CompareArraysOfContracts<T>(T[] contracts1, T[] contracts2)
        {
            return JsonConvert.SerializeObject(contracts1) == JsonConvert.SerializeObject(contracts2);
        }
    }
}