
// Add the namespace containing your Agent entities
using RestAPICore.Common;
using RestAPICore.Entities;
using RestAPICore.Models.Requests;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
//using RestAPICore.Models ;
using RestAPICore.Models.Response;

namespace RestAPICore.Services
{
    public interface IAgentService
    {
        
        
        public AuthAgentResponse checkAgentUserCredentials(AgentAuthRequest request);

     //   public bool checkAgentCredentials(String LoginId, String Password, ref int? agentId);
       
    }
}
