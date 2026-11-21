
using RestAPICore.Auth;
using RestAPICore.Models.Requests;
using RestAPICore.Models.Response;
using RestAPICore.Repositories;
using System.Data;

namespace RestAPICore.Services
{
    public class AgentService : IAgentService
    {
        private readonly IAgentRepository _agentRepository;

        public AgentService(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        //public bool checkAgentCredentials(string loginId, string password, ref int? agentId)
        //{
        //    agentId = null;

        //    if (string.IsNullOrWhiteSpace(loginId) || string.IsNullOrEmpty(password))
        //    {
        //        return false;
        //    }

        //    int? cachedAgentId = _agentRepository.GetAgentIdFromCache(loginId);

        //    if (!cachedAgentId.HasValue)
        //    {
        //        DataSet agents = _agentRepository.GetAgentByLoginId(loginId);
        //        cachedAgentId = FindAgentId(agents, "sub_agent_id");
        //    }

        //    if (!cachedAgentId.HasValue)
        //    {
        //        return false;
        //    }

        //    DataSet agentDetails = _agentRepository.GetAgentDetails(cachedAgentId.Value);
        //    if (!TryGetStoredPassword(agentDetails, out string? storedPassword)
        //        || !string.Equals(password, storedPassword, StringComparison.Ordinal))
        //    {
        //        return false;
        //    }

        //    agentId = cachedAgentId;
        //    return true;
        //}

        public AuthAgentResponse checkAgentUserCredentials(AgentAuthRequest request)
        {
            int? userId = null;

            AccessToken authenticated = _agentRepository.ValidateAgentAndUserCredentials(
                request.LoginId,
                request.Password,
                request.UserType,
                request.SubAgentId,
                ref userId);

            if (!userId.HasValue)
            {
                throw new InvalidOperationException("The authenticated agent user has no user identifier.");
            }

            return new AuthAgentResponse
            {
                accessToken = authenticated.getValue(),
                OldAuth = new AuthAgentResponse.OldAuthDetails
                {
                    Key = MD5(userId.Value.ToString()),
                    UserId = userId.Value
                }
            };
        }

        private string MD5(string password)
        {
            byte[] textBytes = System.Text.Encoding.Default.GetBytes(password);
            try
            {
                System.Security.Cryptography.MD5CryptoServiceProvider cryptHandler;
                cryptHandler = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hash = cryptHandler.ComputeHash(textBytes);
                string ret = "";
                foreach (byte a in hash)
                {
                    if (a < 16)
                        ret += "0" + a.ToString("x");
                    else
                        ret += a.ToString("x");
                }
                return ret;
            }
            catch
            {
                throw;
            }
        }



        //public bool checkAgentCredentials(String LoginId, String Password, ref int? agentId) { 

        //}

        private static int? FindAgentId(DataSet agents, string idColumnName)
        {
            if (agents?.Tables.Count == 0)
            {
                return null;
            }

            foreach (DataRow row in agents!.Tables[0].Rows)
            {
                if (TryGetInt(row, idColumnName, out int agentId))
                {
                    return agentId;
                }
            }

            return null;
        }

        private static int? FindAgentUserId(DataSet agentUsers, string loginId)
        {
            if (agentUsers?.Tables.Count == 0)
            {
                return null;
            }

            foreach (DataRow row in agentUsers!.Tables[0].Rows)
            {
                if (TryGetString(row, "login_id", out string? rowLoginId)
                    && string.Equals(rowLoginId, loginId, StringComparison.OrdinalIgnoreCase)
                    && TryGetInt(row, "sub_agent_user_id", out int userId))
                {
                    return userId;
                }
            }

            return null;
        }

        private static bool TryGetStoredPassword(DataSet details, out string? password)
        {
            password = null;

            if (details?.Tables.Count > 0)
            {
                foreach (DataRow row in details.Tables[0].Rows)
                {
                    if (TryGetString(row, "password", out password))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TryGetString(DataRow row, string columnName, out string? value)
        {
            value = null;

            DataColumn? column = FindColumn(row.Table, columnName);
            if (column == null || row.IsNull(column))
            {
                return false;
            }

            value = Convert.ToString(row[column]);
            return value != null;
        }

        private static bool TryGetInt(DataRow row, string columnName, out int value)
        {
            value = 0;

            DataColumn? column = FindColumn(row.Table, columnName);
            return column != null
                && !row.IsNull(column)
                && int.TryParse(Convert.ToString(row[column]), out value);
        }

        private static DataColumn? FindColumn(DataTable table, string columnName)
        {
            foreach (DataColumn column in table.Columns)
            {
                if (string.Equals(column.ColumnName, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return column;
                }
            }

            return null;
        }
    }
}