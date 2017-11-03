using System;
using System.Collections.Generic;
using ApiContracts.Output;
using Kontur.GameStats.Server.Infrastructure;
using Kontur.GameStats.Server.Logic;
using Kontur.GameStats.Server.Repositories;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Modules
{
    public class GetReportsModule : ModuleBase
    {

        private readonly OutputDataRepository _outputDataRepository;

        public GetReportsModule(OutputDataRepository outputDataRepository) 
        {
            _outputDataRepository = outputDataRepository;

            //A capture node can either be optional or have a constraint, not both at the same time.
            Get["/reports/recent-matches/{count:int}"] = parameters => BuildRecentMatchesReport(parameters.count);
            Get["/reports/recent-matches/"] = parameters => BuildRecentMatchesReport();

            Get["/reports/best-players/{count:int}"] = parameters => BuildBestPlayersReport(parameters.count);
            Get["/reports/best-players/"] = parameters => BuildBestPlayersReport();

            Get["/reports/popular-servers/{count:int}"] = parameters => BuildPopularServersReport(parameters.count);
            Get["/reports/popular-servers/"] = parameters => BuildPopularServersReport();
        }

        private string BuildRecentMatchesReport(int count = CommonConstants.ReportDefaultItemCount)
        {
            if (count <= 0)
                return JsonConvert.SerializeObject(new List<RecentMatchContract>());

            var recentMatches =
                _outputDataRepository.GetRecentMatches(NormalizeCount(count));

            return JsonConvert.SerializeObject(recentMatches);
        }

        private string BuildBestPlayersReport(int count = CommonConstants.ReportDefaultItemCount)
        {
            if (count <= 0)
                return JsonConvert.SerializeObject(new List<BestPlayerContract>());

            var bestPlayers = _outputDataRepository.GetBestPlayers(NormalizeCount(count));

            return JsonConvert.SerializeObject(bestPlayers);
        }

        private string BuildPopularServersReport(int count = CommonConstants.ReportDefaultItemCount)
        {
            if (count <= 0)
                return JsonConvert.SerializeObject(new List<PopularServerContract>());

            var popularServers = _outputDataRepository.GetPopularServers(NormalizeCount(count));

            return JsonConvert.SerializeObject(popularServers);
        }

        private static int NormalizeCount(int count)
        {
            return Math.Min(count, CommonConstants.ReportMaxItemCount);
        }
    }
}