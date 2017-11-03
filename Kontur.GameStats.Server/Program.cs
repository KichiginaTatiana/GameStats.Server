using System;
using Fclp;
using Kontur.GameStats.Server.Infrastructure;
using Kontur.GameStats.Server.Logic;
using log4net;
using Microsoft.Owin.Hosting;
using Nancy.Owin;
using Nancy.TinyIoc;
using Owin;

namespace Kontur.GameStats.Server
{
    internal static class Program
    {
        private class Options
        {
            public string Prefix { get; set; }
            public int TaskPeriodInSec { get; set; }
        }

        private static readonly ILog Log = LogManager.GetLogger(nameof(Program));
        private static TinyIoCContainer _container;

        private static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            AppDomain.CurrentDomain.UnhandledException +=
                (sender, eventArgs) => Log.Error(eventArgs.ExceptionObject);

            var commandLineParser = CreateCommandLineParser();
            if (commandLineParser.Parse(args).HelpCalled)
                return;

            RunServer(commandLineParser.Object);
        }

        private static FluentCommandLineParser<Options> CreateCommandLineParser()
        {
            var commandLineParser = new FluentCommandLineParser<Options>();

            commandLineParser
                .Setup(options => options.Prefix)
                .As("prefix")
                .SetDefault("http://+:8080/")
                .WithDescription("HTTP prefix to listen on");

            commandLineParser
                .Setup(options => options.TaskPeriodInSec)
                .As("taskPeriod")
                .SetDefault(30)
                .WithDescription("Task period in seconds");

            commandLineParser
                .SetupHelp("h", "help")
                .WithHeader($"{AppDomain.CurrentDomain.FriendlyName} [--prefix <prefix> --taskPeriod <taskPeriod>]")
                .Callback(text => Console.WriteLine(text));

            return commandLineParser;
        }

        private static void RunServer(Options o)
        {
            try
            {
                using (WebApp.Start(new StartOptions(o.Prefix), appBuilder =>
                {   
                    var bootstrapper = new NancyBootstrapper();
                    appBuilder.UseNancy(new NancyOptions {Bootstrapper = bootstrapper});
                    _container = bootstrapper.GetContainer();
                }))
                {
                    Log.Info($"Running on {o.Prefix}");
                    Console.WriteLine("Press any key to exit");

                    var taskRunner = _container.Resolve<TaskRunner>();
                    var statisticsCalculator = _container.Resolve<StatisticsCalculator>();

                    taskRunner.Run(() => statisticsCalculator.RecalculateAll(), o.TaskPeriodInSec);

                    Console.ReadKey();
                    Log.Info("Stopped");
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
