
using RestAPICore.DB;

namespace RestAPICore.Entities
{
    public class Agent
    {
        public class CreateUserResponse
        {
            public User[] Users { get; set; }
        }

        public class User
        {
            public string userId { get; set; }
            public string password { get; set; }
            public string name { get; set; }
        }

        public enum SA_TYPE
        {
            AGENT = 1,
            CORPORATE = 2,
            API = 3,
            MICROSITE = 4,
            TRAVELYAARI = 5
        }

        public async static Task<bool> checkClientCredentials(IRedisConnectionFactory redisConnectionFactory, int clientId, string clientSecret)
        {
            Entities.AgentDetails agentDetails = new Entities.AgentDetails(redisConnectionFactory);
            var result = agentDetails.getAgentDetails(agentId: clientId);
            if (result != null && result.client_secret == clientSecret)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int getSubAgentId(string client)
        {
            string[] parts = client.Split('-');
            if (parts[1]=="S")
            {
                return Convert.ToInt32(parts[0]);
            }else
            {
                int agentUserId= Convert.ToInt32(parts[0]);
                AgentUserDetails eAgentUserDetails = new AgentUserDetails();
                var details = eAgentUserDetails.getAgentUserDetails(agentUserId);
                int subAgentId = details.sub_agent_id;
                return subAgentId;
            }
        }
    }


}
