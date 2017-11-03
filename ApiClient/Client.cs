using System.Collections.Generic;
using System.Net;
using ApiContracts.Input;
using ApiContracts.Output;
using Newtonsoft.Json;
using RestSharp;

namespace ApiClient
{
    public class Client
    {
        private readonly RestClient _rc;

        public Client(string url)
        {
            _rc = new RestClient(url);
        }

        public HttpStatusCode PutServer(string endpoint, ServerContract contract)
        {
            return SendPutRequest($"servers/{endpoint}/info", contract);
        }

        public ServerContract GetServer(string endpoint)
        {
            return SendGetRequest<ServerContract>($"servers/{endpoint}/info");
        }

        public IEnumerable<ServerInfoContract> GetServers()
        {
            return SendGetRequest<ServerInfoContract[]>("servers/info");
        }

        public HttpStatusCode PutMatch(string endpoint, string timestamp, MatchContract contract)
        {
            return SendPutRequest($"servers/{endpoint}/matches/{timestamp}", contract);
        }

        public MatchContract GetMatch(string endpoint, string timestamp)
        {
            return SendGetRequest<MatchContract>($"servers/{endpoint}/matches/{timestamp}");
        }

        public ServerStatContract GetServerStat(string endpoint)
        {
            return SendGetRequest<ServerStatContract>($"/servers/{endpoint}/stats");
        }

        public PlayerStatContract GetPlayerStat(string name)
        {
            return SendGetRequest<PlayerStatContract>($"/players/{name}/stats");
        }

        public IEnumerable<RecentMatchContract> GetRecentMatches(int count = 5)
        {
            return SendGetRequest<RecentMatchContract[]>($"/reports/recent-matches/{count}");
        }

        public IEnumerable<BestPlayerContract> GetBestPlayers(int count = 5)
        {
            return SendGetRequest<BestPlayerContract[]>($"/reports/best-players/{count}");
        }

        public IEnumerable<PopularServerContract> GetPopularServers(int count = 5)
        {
            return SendGetRequest<PopularServerContract[]>($"/reports/popular-servers/{count}");
        }

        private HttpStatusCode SendPutRequest<T>(string resource, T contract)
        {
            var request = new RestRequest(resource, Method.PUT);
            request.AddJsonBody(contract);
            var response = _rc.Execute(request);

            return response.StatusCode;
        }

        private T SendGetRequest<T>(string resource)
        {
            var request = new RestRequest(resource, Method.GET);
            var response = _rc.Execute(request);

            return response.StatusCode == HttpStatusCode.OK
                ? JsonConvert.DeserializeObject<T>(response.Content)
                : default(T);
        }
    }
}
