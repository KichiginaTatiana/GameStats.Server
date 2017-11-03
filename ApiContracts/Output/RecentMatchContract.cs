using ApiContracts.Input;

namespace ApiContracts.Output
{
    public class RecentMatchContract
    {
        public string timestamp { get; set; }
        public string serverEndpoint { get; set; }
        public MatchContract results { get; set; }
}
}
