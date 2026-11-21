using RestAPICore.Auth;
using RestAPICore.Models;
//using RestAPICore.Repositories;
using RestAPICore.Repositories.AuthRepo;

namespace RestAPICore.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;

        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public Task<string?> AuthenticateAsync(AuthRequest request)
        {
            if (request.ClientId <= 0 || string.IsNullOrWhiteSpace(request.ClientSecret))
            {
                return Task.FromResult<string?>(null);
            }

            var userId = request.UserId ?? Authorization.ADMIN_USER_ID;
            return AuthByKeyAsync(request.ClientId, request.ClientSecret, userId);
        }

        public async Task<string?> AuthByKeyAsync(int clientId, string clientSecret, string userId)
        {
            if (clientId <= 0 || string.IsNullOrWhiteSpace(clientSecret))
            {
                return null;
            }

            var isValid = await _authRepository.ValidateClientCredentialsAsync(clientId, clientSecret);
            if (!isValid)
            {
                return null;
            }

            return AccessToken.Create($"{clientId}-S", userId).getValue();
        }

        //public string? AuthAgent(string loginId, string password, int? userSubAgentId, string clientType, ref int? userId)
        //{
        //    bool success;

        //    switch (clientType)
        //    {
        //        case "S":
        //            success = _authRepository.CheckAgentCredentials(loginId, password, ref userId);
        //            break;
        //        case "U":
        //            success = userSubAgentId.HasValue
        //                && _authRepository.CheckAgentUserCredentials(userSubAgentId.Value, loginId, password, ref userId);
        //            break;
        //        default:
        //            throw new ArgumentException("Invalid User Type !", nameof(clientType));
        //    }

        //    if (!success || userId == null)
        //    {
        //        throw new UnauthorizedAccessException("Invalid credentials");
        //    }

        //    return AccessToken.Create($"{userId}-{clientType}").getValue();
        //}

        //public string? AuthAgentV2(string loginId, string password, int? userSubAgentId, string clientType, ref int? userId)
        //{
        //    switch (userSubAgentId)
        //    {
        //        case 9542:
        //            clientType = loginId == "mantistech" ? "A" : "S";
        //            break;
        //        default:
        //            clientType = "U";
        //            break;
        //    }

        //    bool success;
        //    if (clientType == "U")
        //    {
        //        success = userSubAgentId.HasValue
        //            && _authRepository.CheckAgentUserCredentials(userSubAgentId.Value, loginId, password, ref userId);
        //    }
        //    else
        //    {
        //        success = _authRepository.CheckAgentCredentials(loginId, password, ref userId);
        //    }

        //    if (!success || userId == null || userId == 0)
        //    {
        //        throw new UnauthorizedAccessException("Invalid credentials");
        //    }

        //    return AccessToken.Create($"{userId}-{clientType}").getValue();
        //}

        //public string AuthInternalClient(int userId, string userType)
        //{
        //    return AccessToken.Create($"{userId}-{userType}").getValue();
        //}
    }
}
