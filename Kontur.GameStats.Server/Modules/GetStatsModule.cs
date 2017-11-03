using System.Web;
using Kontur.GameStats.Server.Infrastructure;
using Kontur.GameStats.Server.Repositories;
using Nancy;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Modules
{
    public class GetStatsModule : ModuleBase
    {
        public GetStatsModule(OutputDataRepository outputDataRepository) 
        {
            Get["/servers/{endpoint}/stats"] = parameters =>
            {
                var result = outputDataRepository.GetServerStat(parameters.endpoint);
                if (result == null)
                    return HttpStatusCode.NotFound; 

                return JsonConvert.SerializeObject(result);
            };

            Get["/players/{name}/stats"] = parameters =>
            {
                var result = outputDataRepository.GetPlayerStat(HttpUtility.UrlDecode(parameters.name));
                if (result == null)
                    return HttpStatusCode.NotFound;

                return JsonConvert.SerializeObject(result);
            };
        }
    }
}