
using RestAPICore.DB;
using System.Text.Json;

namespace RestAPICore.Entities
{
    public class AgentDetails : BaseEntity
    {
        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public AgentDetails(IRedisConnectionFactory redisConnectionFactory) : base("agent1")
        {
            _redisConnectionFactory = redisConnectionFactory ?? throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        

        public class AgentFields
        {
        public int client_id { get; set; }
        public string client_secret { get; set; }
        public int sub_agent_id { get; set; }
        public string sub_agent_name { get; set; }
        public string login_id { get; set; }
        public string password { get; set; }
        public bool is_active { get; set; }
        public string email_id { get; set; }
        public decimal comm_pct { get; set; }
        public decimal? comm_pct_ac { get; set; } = null;
        public decimal balance_oth { get; set; }
        public decimal comm_pct_lower { get; set; }
        public decimal comm_pct_lower_ac { get; set; }
        public decimal service_charge_sit_nac { get; set; }
        public decimal service_charge_sit_ac { get; set; }
        public decimal service_charge_slp_nac { get; set; }
        public decimal service_charge_slp_ac { get; set; }
        public bool is_prepaid { get; set; }
        public string mobile_for_updates { get; set; }
        public int commission_rule { get; set; }
        public int sa_type { get; set; }
        public int? visibility { get; set; } = 4;
        public bool restrict_ip { get; set; } = true;
        public string reg_email { get; set; }
        public int[] prov_ids { get; set; }
        public decimal cancel_pct { get; set; }
        //public int parentSubAgentId { get; set; } = 8833; // one level up entity-id  for eg: for a sub_agent one level up is distributor.
        //public bool isRegularAgent { get; set; } = true;
        public int parentSubAgentId { get; set; } // one level up entity-id  for eg: for a sub_agent one level up is distributor.
        public int isRegularAgent { get; set; }
        }

        public AgentFields getAgentDetails(int agentId)
        {
            string[] eVars = new string[] { agentId.ToString() };

            string redisKey = getRedisKey(eVars);

            var client = _redisConnectionFactory.GetDatabase();

            string result = client.StringGet(redisKey);

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            return  JsonSerializer.Deserialize<AgentFields>(
    result,
    new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });
        }

        public void saveAgentDetails(int agentId, Dictionary<string, object> details)
        {
            string redisKey = getRedisKey(new[] { agentId.ToString() });
            string json = Jil.JSON.Serialize(details);
            var client = _redisConnectionFactory.GetDatabase();
            client.StringSet(redisKey, json);
        }
    }
}
