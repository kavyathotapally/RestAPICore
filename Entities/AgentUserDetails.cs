using RestAPICore.DB;
using RestAPICore.Repositories.CityRepo;
using System;
using System.Text.Json;

namespace RestAPICore.Entities
{
    public class AgentUserDetails : BaseEntity
    {
        private readonly IRedisConnectionFactory redisRepository;

        public AgentUserDetails(IRedisConnectionFactory redisRepository)
            : base("agentuserdetails1")
        {
            this.redisRepository = redisRepository
                ?? throw new ArgumentNullException(nameof(redisRepository));
        }
        public AgentUserDetails() : base("agentuserdetails1")
        {

        }
        public class AgentUserFields
        {
            public int sub_agent_user_id { get; set; }
            public int sub_agent_id { get; set; }
            public string login_id { get; set; }
            public string password { get; set; }
            public string sub_agent_user_name { get; set; }
            public bool is_active { get; set; }
            public bool is_regular_user { get; set; }
            public int commission_rule { get; set; }
            public decimal comm_share_pct { get; set; }
            public decimal comm_cap_pct { get; set; }
        }

        public AgentUserFields getAgentUserDetails(int agentUserId)
        {
            string[] eVars = new string[] { agentUserId.ToString() };

            string redisKey = this.getRedisKey(eVars);

            var client = redisRepository.GetDatabase(
                REDIS.RedisServer.GDS,
                REDIS.SlaveOrMaster.MASTER);

            string result = client.StringGet(redisKey);

            if (string.IsNullOrEmpty(result))
            {
                return null;
            }

            return JsonSerializer.Deserialize<AgentUserFields>(result);
        }
    }
}
