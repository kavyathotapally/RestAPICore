using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;

//using RestAPICore.Models;
using RestAPICore.Models.Requests;
using RestAPICore.Models.Response;
using RestAPICore.Services;

namespace RestAPICore.Controllers.AgentControllers
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

        
        
    }
}
