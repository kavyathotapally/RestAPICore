using RestAPICore.Models;

namespace RestAPICore.Services.AuthServices
{
    public interface IAuthService
    {
        Task<string?> AuthenticateAsync(AuthRequest request);
        Task<string?> AuthByKeyAsync(int clientId, string clientSecret, string userId);
        //string? AuthAgent(string loginId, string password, int? userSubAgentId, string clientType, ref int? userId);
        //string? AuthAgentV2(string loginId, string password, int? userSubAgentId, string clientType, ref int? userId);
        //string AuthInternalClient(int userId, string userType);
    }
}
