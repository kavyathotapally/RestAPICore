namespace RestAPICore.Repositories.AuthRepo
{
    public interface IAuthRepository
    {
        Task<bool> ValidateClientCredentialsAsync(int clientId, string clientSecret);
       

            
    }
}
