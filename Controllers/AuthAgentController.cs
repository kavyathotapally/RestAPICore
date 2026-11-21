using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;

//using RestAPICore.Models;
using RestAPICore.Models.Requests;
using RestAPICore.Models.Response;
using RestAPICore.Services;

namespace RestAPICore.Controllers
{
   
    public class AuthAgentController : AgentControllerBase
    {
        private readonly IAgentService _agentService;

        public AuthAgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpPost("Auth")]
        public async Task<AuthAgentResponse> Post(AgentAuthRequest request)
        {
            int? userId = null;
            Models.Response.AuthAgentResponse authAgentResponse = _agentService.checkAgentUserCredentials(request);
            return authAgentResponse;
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
    }
}
