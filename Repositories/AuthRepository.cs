

using RestAPICore.DB;

namespace RestAPICore.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IRedisConnectionFactory _redisConnectionFactory;

        public AuthRepository(IRedisConnectionFactory redisConnectionFactory)
        {
            _redisConnectionFactory = redisConnectionFactory ?? throw new ArgumentNullException(nameof(redisConnectionFactory));
        }

        public Task<bool> ValidateClientCredentialsAsync(int clientId, string clientSecret)
        {
            return Entities.Agent.checkClientCredentials(_redisConnectionFactory, clientId, clientSecret);
        }

        //public bool CheckAgentCredentials(string loginId, string password, ref int? agentId)
        //{
        //    return Domain.Agent.checkAgentCredentials(loginId, password, ref agentId);
        //}

        //public bool CheckAgentUserCredentials(int agentId, string loginId, string password, ref int? agentUserId)
        //{
        //    return Domain.Agent.checkAgentUserCredentials(agentId, loginId, password, ref agentUserId);
        //}
    }
}
