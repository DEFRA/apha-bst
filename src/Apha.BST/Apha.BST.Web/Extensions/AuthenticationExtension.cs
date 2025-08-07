using Apha.BST.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace Apha.BST.Web.Extensions
{
    public static class AuthenticationExtension
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
             .AddMicrosoftIdentityWebApp(options =>
             {
                 configuration.Bind("AzureAd", options);
                 options.Events = new OpenIdConnectEvents
                 {
                     OnTokenValidated = context => HandleTokenValidatedAsync(context),
                     OnRedirectToIdentityProvider = context => HandleRedirectToIdentityProvider(context),
                     OnRemoteFailure = context => HandleRemoteFailure(context)
                 };

             });

            return services;
        }

        private static async Task HandleTokenValidatedAsync(TokenValidatedContext context)
        {
            var identity = context.Principal?.Identity as ClaimsIdentity;
            if (identity == null)
            {
                context.Fail("Invalid identity");
                return;
            }
            var email = identity.FindFirst(ClaimTypes.Email)?.Value
                      ?? identity.FindFirst("preferred_username")?.Value;
            if (!string.IsNullOrEmpty(email))
            {
                var accessControlService = context.HttpContext.RequestServices.GetRequiredService<IAccessControlService>();
                var roleMappingService = context.HttpContext.RequestServices.GetRequiredService<IRoleMappingService>();
                var result = await accessControlService.GetRoleIdAndUsernameByEmailAsync(email);

                if (result?.RoleId != null)
                {
                    string roleName = await roleMappingService.GetRoleName(result.Value.RoleId.Value);
                    string username = result.Value.Username!;

                    identity.AddClaim(new Claim("rolename", roleName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, username.ToString()));
                    identity.AddClaim(new Claim("roleid", result.Value.RoleId.Value.ToString()));
                }
                else
                {
                    context.Fail("User Role Missing");
                    return;

                }

            }
            else
            {
                context.Fail("Missing email");
                return;
            }
        }

        /// <summary>
        /// Ensures the redirect URI uses HTTPS instead of HTTP.
        /// This is typically used in OpenID Connect authentication
        /// to enforce secure redirection to the identity provider.
        /// </summary>
        /// <param name="context">
        /// The <see cref="RedirectContext"/> provided by the OIDC middleware.
        /// </param>
        /// <returns>A completed task.</returns>
        private static Task HandleRedirectToIdentityProvider(RedirectContext context)
        {
            Console.WriteLine($"Redirect URL : {context.ProtocolMessage.RedirectUri}");
            // Check if the redirect URI starts with "http://"
            if (context.ProtocolMessage.RedirectUri?.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == true)
            {
                // Replace "http://" with "https://"
                context.ProtocolMessage.RedirectUri =
                    string.Concat("https://", context.ProtocolMessage.RedirectUri.AsSpan("http://".Length));
                Console.WriteLine($"Redirect URL After replace : {context.ProtocolMessage.RedirectUri}");

            }
            return Task.CompletedTask;
        }

        private static Task HandleRemoteFailure(RemoteFailureContext context)
        {
            var errorMessage = context.Failure?.Message ?? "Unknown authentication error.";
            var errorUrl = $"/Error/AccessDenied?error={Uri.EscapeDataString(errorMessage)}";

            context.Response.Redirect(errorUrl);
            context.HandleResponse(); // Prevents default handling
            return Task.CompletedTask;
        }
    }
}
