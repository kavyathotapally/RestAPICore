
using RestAPICore.Common;
using RestAPICore.DB;
using RestAPICore.Entities;
using StackExchange.Redis;
using System.Data;

namespace RestAPICore.Repositories
{
    public class AgentRepository : IAgentRepository
    {
        private readonly MySqlGDSDB _mysql;
      //  private readonly IRedisConnectionFactory _redis;

        public AgentRepository(
            MySqlGDSDB mysql
           )
        {
            _mysql = mysql;
     //       _redis = redis;
        }


        public AccessToken ValidateAgentAndUserCredentials(string LoginId, string Password, string clientType, int? userSubAgentId, ref int? userId)
        {
            var param = new Dictionary<string, object>

            
    {
        { "LOGIN_ID", LoginId },
        { "PASSWORD", Password }
    
       
    };

            switch (userSubAgentId)
            {
                case 9542:
                    if (LoginId == "mantistech")
                    {
                        clientType = "A";
                    }
                    else
                    {
                        clientType = "S";
                    }
                    break;
                default:
                    clientType = "U";
                    break;
            }

            param.Add("USER_TYPE", clientType);
            param.Add("SUB_AGENT_ID", userSubAgentId);
            //  int? userId = null;
            DataSet ds= _mysql.execute("SP_VALIDATE_AGENT_AND_USER_CREDENTIALS_V2", param, CommandType.StoredProcedure);
            
            bool success = false;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                System.Data.DataRow dr = ds.Tables[0].Rows[0];
                success = true;
                userId = Convert.ToInt32(dr["CLIENT_ID"]);
            }

            if (success && userId != null && userId != 0)
            {
                string clientId = userId.ToString() + "-" + clientType;
                AccessToken accessToken = AccessToken.Create(clientId);
                return accessToken;
            }
            else
            {
                throw new Exception("Invalid credentials");
            }
        }

        public DataSet GetAgentByLoginId(string loginId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "LOGIN_ID", loginId }
            };

            return _mysql.execute(
                "SELECT sub_agent_id FROM sub_agent_master WHERE login_id = @p_LOGIN_ID",
                parameters,
                CommandType.Text);
        }

        public DataSet GetAllActiveAgents()
        {
            return _mysql.execute(
                "SELECT DISTINCT sam.sub_agent_id FROM sub_agent_master sam INNER JOIN sub_agent_users sau ON sau.sub_agent_id = sam.sub_agent_id WHERE sam.is_active = 1",
                new Dictionary<string, object>(),
                CommandType.Text);
        }

        public DataSet GetAgentUsers(int agentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "AGENT_ID", agentId }
            };

            return _mysql.execute(
                "SELECT sub_agent_user_id, login_id FROM sub_agent_users WHERE sub_agent_id = @p_AGENT_ID",
                parameters,
                CommandType.Text);
        }

        public DataSet GetAgentDetails(int agentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "client_id", agentId },
                { "user_type", null }
            };

            return _mysql.execute(
                "REST_GET_CLIENT_DETAILS_V1",
                parameters);
        }

        public DataSet GetAgentUserDetails(int agentUserId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "client_id", agentUserId },
                { "user_type", "U" }
            };

            return _mysql.execute(
                "REST_GET_CLIENT_DETAILS",
                parameters);
        }

        public DataSet GetAgentSplComm(int agentId)
        {
            var parameters = new Dictionary<string, object>
            {
                { "client_id", agentId }
            };

            return _mysql.execute(
                "REST_GET_CLIENT_SPL_COMM_V1",
                parameters);
        }

        public DataSet GetClientDetails(int clientId, string userType)
        {
            var parameters = new Dictionary<string, object>
            {
                { "client_id", clientId },
                { "user_type", userType }
            };

            return _mysql.execute(
                "REST_GET_CLIENT_DETAILS",
                parameters);
        }

     
    }
}
