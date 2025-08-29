using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class ErrorControllerTests
    {
        private ErrorController SetupController()
        {
            var services = new ServiceCollection();

            // Add logging if your controller or framework requires it
            services.AddLogging();

            // Add TempData services:
            services.AddSingleton<ITempDataProvider, SessionStateTempDataProvider>();
            services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();

            // Add a minimal MVC infrastructure for Controllers and Views:
            services.AddControllersWithViews();

            var serviceProvider = services.BuildServiceProvider();

            var controller = new ErrorController();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            return controller;
        }

        [Fact]
        public void Index_NoException_ReturnsDefaultErrorMessage()
        {
            // Arrange
            var controller = SetupController();

            // Mock Features to return null for IExceptionHandlerPathFeature
            var features = Substitute.For<IFeatureCollection>();
            features.Get<IExceptionHandlerPathFeature>().Returns((IExceptionHandlerPathFeature?)null);

            // Replace HttpContext to mock Features (but keep RequestServices)
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Features.Returns(features);

            // Keep RequestServices so TempDataFactory works
            httpContext.RequestServices.Returns(controller.HttpContext.RequestServices);

            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("An unexpected error occurred. Please try again later.", model.ErrorMessage);
        }

        [Fact]
        public void Index_WithException_ReturnsExceptionMessage()
        {
            // Arrange
            var controller = SetupController();

            var exceptionFeature = Substitute.For<IExceptionHandlerPathFeature>();
            exceptionFeature.Error.Returns(new Exception("Test error occurred."));

            var features = Substitute.For<IFeatureCollection>();
            features.Get<IExceptionHandlerPathFeature>().Returns(exceptionFeature);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Features.Returns(features);
            httpContext.RequestServices.Returns(controller.HttpContext.RequestServices);

            controller.ControllerContext.HttpContext = httpContext;

            // Act
            var result = controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("Test error occurred.", model.ErrorMessage);
        }
    }
}
