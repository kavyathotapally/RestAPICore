using RestAPICore.DB;

namespace RestAPICore.Repositories.AuthRepo
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

      
    }
}
