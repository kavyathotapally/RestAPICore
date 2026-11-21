namespace RestAPICore.Repositories
{
    public interface IAuthRepository
    {
        Task<bool> ValidateClientCredentialsAsync(int clientId, string clientSecret);
        //bool CheckAgentCredentials(string loginId, string password, ref int? agentId);
        //bool CheckAgentUserCredentials(int agentId, string loginId, string password, ref int? agentUserId);

            
    }
}
