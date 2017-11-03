using System;
using System.Text;
using ApiContracts.Input;
using Kontur.GameStats.Server.Infrastructure;
using Kontur.GameStats.Server.Logic;
using Kontur.GameStats.Server.Repositories;
using Nancy;
using Newtonsoft.Json;

namespace Kontur.GameStats.Server.Modules
{
    public class PutModule : ModuleBase
    {
        public PutModule(InputDataRepository inputDataRepository, DateHelper dateHelper) 
        {
            Put["/servers/{endpoint}/info"] = parameters =>
            {
                ServerContract contract;
                if (!TryDeserialize(out contract))
                    return HttpStatusCode.BadRequest;

                inputDataRepository.PutServerInfo(parameters.endpoint, contract);
                return HttpStatusCode.OK;
            };

            Put["/servers/{endpoint}/matches/{timestamp}"] = parameters =>
            {
                if (!dateHelper.CheckTimestamp(parameters.timestamp))
                    return HttpStatusCode.BadRequest;

                MatchContract contract;
                if (!TryDeserialize(out contract))
                    return HttpStatusCode.BadRequest;

                return inputDataRepository.PutMatchInfo(parameters.endpoint, parameters.timestamp, contract)
                    ? HttpStatusCode.OK
                    : HttpStatusCode.BadRequest;
            };
        }

        private bool TryDeserialize<T>(out T contract)
        {
            contract = default(T);

            var json = ExtractJson();
            if (string.IsNullOrEmpty(json))
                return false;

            try
            {
                contract = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception e)
            {
                Log.Error(e);
                return false;
            }

            return contract != null;
        }

        private string ExtractJson()
        {
            var body = Request.Body;
            var length = (int) body.Length;
            var data = new byte[length];
            body.Read(data, 0, length);
            return Encoding.Default.GetString(data);
        }
    }
}
