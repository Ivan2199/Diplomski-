using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using GeoTagMap.Common;
using GeoTagMap.Models.Common;
using GeoTagMap.Service.Common;
using GeoTagMap.WebApi;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;

[EnableCors(origins: "*", headers: "*", methods: "*")]
public class ApplicationAuthProvider : OAuthAuthorizationServerProvider
{
    private readonly IUserService _userService;

    public ApplicationAuthProvider(IUserService userService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
    {
        context.Validated();
    }

    public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
    {
        try
        {
            IUserModel user = await _userService.ValidateUserAsync(new UserFilter(context.UserName, context.Password));

            if (user != null)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.UserRole.Type));
                identity.AddClaim(new Claim("UserId", user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

                var props = new AuthenticationProperties
                {
                    IssuedUtc = context.Options.SystemClock.UtcNow,
                    ExpiresUtc = context.Options.SystemClock.UtcNow.Add(context.Options.AccessTokenExpireTimeSpan),
                    AllowRefresh = true,
                };

                var ticket = new AuthenticationTicket(identity, props);

                var accessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);

                var userId = identity.FindFirst("UserId")?.Value;
                var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
                var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;
                var userRole = identity.FindFirst(ClaimTypes.Role)?.Value;

                var responseData = new
                {
                    access_token = accessToken,
                    user = new
                    {
                        username = userName,
                        email = userEmail,
                        userRole = userRole,
                        userId = userId
                    }
                };

                context.Response.ContentType = "application/json";
                context.Response.Write(JsonConvert.SerializeObject(responseData));

                context.Validated(ticket);
            }
            else
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                context.SetError("error_description", "Invalid username or password. Please try again.");
                return;
            }
        }
        catch (Exception ex)
        {
            context.SetError("server_error", ex.Message);
            context.SetError("error_description", "Server error occurred. Please try again later.");
            return;
        }
    }
}