using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Apha.BST.Application.Interfaces;

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
                     OnTokenValidated = async context =>
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
                     },
                     OnRemoteFailure = context =>
                     {
                         var errorMessage = context.Failure?.Message ?? "Unknown authentication error.";
                         var errorUrl = $"/Error/AccessDenied?error={Uri.EscapeDataString(errorMessage)}";

                         context.Response.Redirect(errorUrl);
                         context.HandleResponse(); // Prevents default handling
                         return Task.CompletedTask;
                     }
                 };

             });

            return services;
        }
    }
}
