using Microsoft.AspNetCore.Mvc;
using RestAPICore.Controllers.BaseControllers;
using RestAPICore.Models;
using RestAPICore.Services;

namespace RestAPICore.Controllers.TYControllers
{
   
    public class AuthTYController : TYControllerBase
    {
        private readonly IAuthService _authService;

        public AuthTYController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Auth")]
        public async Task<IActionResult> Post(AuthRequest request)
        {
            if (request.ClientId == 0 || string.IsNullOrEmpty(request.ClientSecret))
            {
                return BadRequest("Credentials can not be empty !");
            }

            var accessToken = await _authService.AuthenticateAsync(request);
            if (accessToken == null)
            {
                return Unauthorized("Invalid Credentials");
            }

            return Ok(new AuthAgentResponse { accessToken = accessToken });
        }
    }
}
