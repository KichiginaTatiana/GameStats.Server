using Kontur.GameStats.Server.Infrastructure;
using Kontur.GameStats.Server.Logic;
using Kontur.GameStats.Server.Repositories;
using Nancy;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Modules
{
    public class GetInfoModule : ModuleBase
    {
        public GetInfoModule(InputDataRepository inputDataRepository, DateHelper dateHelper) 
        {
            Get["/servers/{endpoint}/info"] = parameters =>
            {
                var server = inputDataRepository.GetServerInfo(parameters.endpoint);
                if (server == null)
                    return HttpStatusCode.NotFound;

                return JsonConvert.SerializeObject(server);
            };

            Get["/servers/info"] = _ =>
            {
                var servers = inputDataRepository.GetServerInfos();
                if (servers == null)
                    return HttpStatusCode.NotFound;

                return JsonConvert.SerializeObject(servers);
            };

            Get["/servers/{endpoint}/matches/{timestamp}"] = parameters =>
            {
                if (!dateHelper.CheckTimestamp(parameters.timestamp))
                    return HttpStatusCode.BadRequest;

                var match = inputDataRepository.GetMatchesInfo(parameters.endpoint, parameters.timestamp);
                if (match == null)
                    return HttpStatusCode.NotFound;

                return JsonConvert.SerializeObject(match);
            };
        }
    }
}
