
using RestAPICore.Auth;
using RestAPICore.Entities;
using System.Collections.Generic;
using System.Data;

namespace RestAPICore.Repositories
{
    public interface IAgentRepository
    {
        public AccessToken ValidateAgentAndUserCredentials(string loginId, string password, string clientType, int? userSubAgentId, ref int?userId);
    } 
}
