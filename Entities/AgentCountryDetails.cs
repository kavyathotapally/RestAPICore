using RestAPICore.DB;
using RestAPICore.Models;
using System.Text.Json;
using static RestAPICore.Entities.AgentDetails;

namespace RestAPICore.Entities
{
    public class AgentCountryDetails: BaseEntity
    {

        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public AgentCountryDetails(IRedisConnectionFactory redisConnectionFactory) : base("agentcountry1")
        {
            _redisConnectionFactory = redisConnectionFactory ?? throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        

        public AgentCountryFields[]   getAgentCountryDetails(int agentId)
        {
            string[] eVars = new string[] { agentId.ToString() };
            string redisKey = this.getRedisKey(eVars);
            string result = null;
            var client = _redisConnectionFactory.GetDatabase();
            result = client.StringGet(redisKey);
            if (string.IsNullOrEmpty(result))
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<AgentCountryFields[]>( result, new JsonSerializerOptions {  PropertyNameCaseInsensitive = true });
            }
        }

       

        //public void saveAgentCountryDetails(int agentId, List<AgentCountryFields> agentCountryList)
        //{
        //    string[] eVars = new string[] { agentId.ToString() };
        //    string redisKey = this.getRedisKey(eVars);
        //    var client = _redisConnectionFactory.GetDatabase();
        //    string val = Jil.JSON.SerializeDynamic(agentCountryList);
        //    client.StringSet(redisKey, val);
        //}

        public int[] getAgentCountryIds(int? agentId)
        {
            int[] countryIds = null;
            if (agentId != null)
            {
                int aid = agentId.Value;
                AgentCountryFields[] af = this.getAgentCountryDetails(aid);
                if (af != null && af.Length > 0)
                {
                    countryIds = new int[af.Length];
                    for (int i = 0; i < af.Length; i++)
                    {
                        countryIds[i] = af[i].cId;
                    }
                }
            }
            return countryIds;
        }
    }

}

