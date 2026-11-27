namespace RestAPICore.Models.Response
{
    public class CityList
    {
        public AgentCityItem[] Cities { get; set; } = Array.Empty<AgentCityItem>();

        public string[] AllStates { get; set; } = Array.Empty<string>();

        public string[] TyCities { get; set; } = Array.Empty<string>();
    }
}
