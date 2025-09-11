using Apha.BST.DataAccess.Data;
using Apha.BST.Web.Extensions;
using Apha.BST.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Web.UnitTests.Extensions
{
    public class ProgramExtensionTests
    {
        [Fact]
        public void ConfigureServices_ShouldRegisterServices()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "ConnectionStrings:BSTConnectionString", "Server=(localdb)\\mssqllocaldb;Database=FakeDb;Trusted_Connection=True;" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddConfiguration(configuration);

            // Act
            builder.ConfigureServices();

            // Assert
            var serviceProvider = builder.Services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<IHttpContextAccessor>());
        }

        [Fact]
        public void ConfigureMiddleware_ShouldConfigureWithoutThrowing()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string?>
            {
                { "DefaultCulture", "en-GB" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var builder = WebApplication.CreateBuilder();
            builder.Configuration.AddConfiguration(configuration);

            builder.ConfigureServices();
            var app = builder.Build();

            // Act + Assert
            var exception = Record.Exception(() => app.ConfigureMiddleware());
            Assert.Null(exception);
        }
    }
}
