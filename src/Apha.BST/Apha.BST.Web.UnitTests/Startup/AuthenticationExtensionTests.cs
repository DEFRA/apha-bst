using Apha.BST.Web.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using NSubstitute;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Apha.BST.Application.Interfaces;

namespace Apha.BST.Web.UnitTests.Startup
{
    public class AuthenticationExtensionTests
    {
        private readonly IServiceCollection _services;
        private readonly IConfiguration _configuration;

        public AuthenticationExtensionTests()
        {
            _services = new ServiceCollection();
            _configuration = Substitute.For<IConfiguration>();
        }

        [Fact]
        public void AddAuthenticationServices_ShouldRegisterAuthentication()
        {
            // Act
            var result = AuthenticationExtension.AddAuthenticationServices(_services, _configuration);

            // Assert
            Assert.NotNull(result);
            var serviceProvider = _services.BuildServiceProvider();
            var authService = serviceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationService>();
            Assert.NotNull(authService); // Basic check - deeper checks can be added
        }
     }

}
