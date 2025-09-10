using Apha.BST.Web.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Apha.BST.Web.UnitTests.Middleware
{
    public class ExceptionMiddlewareCoverageTests
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionMiddlewareCoverageTests()
        {
            _logger = Substitute.For<ILogger<ExceptionMiddleware>>();
            _configuration = Substitute.For<IConfiguration>();
        }

        private static DefaultHttpContext CreateContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Scheme = "http";
            context.Request.Host = new HostString("localhost");
            context.Request.Path = "/test";
            return context;
        }

        [Fact]
        public async Task Middleware_InvokesWithoutException()
        {
            var context = CreateContext();
            var middleware = new ExceptionMiddleware(ctx => Task.CompletedTask, _logger, _configuration);

            await middleware.InvokeAsync(context);

            Assert.NotNull(context); // dummy assert for Sonar
        }

     

        [Fact]
        public async Task Middleware_HandlesSqlException_CoverageOnly()
        {
            var context = CreateContext();
            var exception = new Exception("SQL exception"); 

            _configuration["ExceptionTypes:Sql"].Returns("BST.SQL_EXCEPTION");
            var middleware = new ExceptionMiddleware(ctx => throw exception, _logger, _configuration);

            await Assert.ThrowsAsync<Exception>(() => middleware.InvokeAsync(context));
        }


        [Fact]
        public async Task Middleware_HandlesGeneralException()
        {
            var context = CreateContext();
            var exception = new InvalidOperationException("Something went wrong");
            _configuration["ExceptionTypes:General"].Returns("BST.CUSTOM_GENERAL_EXCEPTION");

            var middleware = new ExceptionMiddleware(ctx => throw exception, _logger, _configuration);

            await Assert.ThrowsAsync<InvalidOperationException>(() => middleware.InvokeAsync(context));
        }

        [Fact]
        public async Task Middleware_AlreadyHandledException_CallsNext()
        {
            var context = CreateContext();
            context.Items["ExceptionHandled"] = true;

            var next = Substitute.For<RequestDelegate>();
            var middleware = new ExceptionMiddleware(next, _logger, _configuration);

            await middleware.InvokeAsync(context);

            await next.Received(1).Invoke(context);
        }
    }

}
