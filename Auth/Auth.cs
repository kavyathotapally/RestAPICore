using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RestAPICore.Auth
{
    public class Auth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;

            // Read access-token header
            var token = request.Headers["access-token"].FirstOrDefault();

            // Check Authorization: Bearer header
            if (string.IsNullOrEmpty(token))
            {
                var authorization = request.Headers.Authorization.FirstOrDefault();

                if (!string.IsNullOrEmpty(authorization) &&
                    authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    token = authorization.Substring("Bearer ".Length).Trim();
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new UnauthorizedObjectResult(
                    "Access token not passed!"
                );
                return;
            }
            try
            {
                AuthPrincipal principal = new AuthPrincipal(token);
                context.HttpContext.Items["AuthPrincipal"] = principal;
            }
            catch (System.Exception ex)
            {
                context.Result = new UnauthorizedObjectResult(ex.Message);
            }
            // TODO: Validate the access token here.
        }
    }
}