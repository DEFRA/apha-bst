using Apha.BST.Web.Controllers;
using Apha.BST.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Web.UnitTests.Controllers
{
    public class HelpControllerTests
    {
        private readonly HelpController _controller;

        public HelpControllerTests()
        {
            _controller = new HelpController();
        }

        [Fact]
        public void Help_ReturnsViewResult()
        {
            // Act
            var result = _controller.Help();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        public void Help_ReturnsCorrectViewModel(int index)
        {
            // Act
            var result = _controller.Help(index) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<HelpViewModel>(result!.Model);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        public void Help_SetsCorrectActiveViewIndex(int index)
        {
            // Act
            var result = _controller.Help(index) as ViewResult;
            Assert.NotNull(result);
            var model = result!.Model as HelpViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(index, model!.ActiveViewIndex);
        }

        [Fact]
        public void Help_ReturnsCorrectNumberOfSections()
        {
            // Act
            var result = _controller.Help() as ViewResult;
            Assert.NotNull(result);
            var model = result!.Model as HelpViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(8, model!.Sections.Count);
        }

        [Fact]
        public void Help_ReturnsSectionsWithCorrectContent()
        {
            // Act
            var result = _controller.Help() as ViewResult;
            Assert.NotNull(result);
            var model = result!.Model as HelpViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Contains(model!.Sections, s => s.Title == "User help" && s.ImageUrl == "/Images/BSTUserAdd.PNG");
            Assert.Contains(model.Sections, s => s.Title == "Site help" && s.ImageUrl == "/Images/BSTSite.PNG");
            Assert.Contains(model.Sections, s => s.Title == "Home help" && s.ImageUrl == "/Images/BSThome.PNG");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Help_WithNegativeIndex_ReturnsViewModelWithZeroIndex(int negativeIndex)
        {
            // Act
            var actionResult = _controller.Help(negativeIndex);

            // Assert
            Assert.IsType<ViewResult>(actionResult);
            var viewResult = (ViewResult)actionResult;

            Assert.IsType<HelpViewModel>(viewResult.Model);
            var model = (HelpViewModel)viewResult.Model;

            Assert.Equal(0, model.ActiveViewIndex);
        }

        [Fact]
        public void Help_WithDefaultParameter_ReturnsViewModelWithZeroIndex()
        {
            // Act
            var result = _controller.Help() as ViewResult;
            Assert.NotNull(result);
            var model = result!.Model as HelpViewModel;

            // Assert
            Assert.NotNull(model);
            Assert.Equal(0, model!.ActiveViewIndex);
        }

        [Fact]
        public void Help_DoesNotCallExternalSystems()
        {
            // This test is to ensure that no external systems are called.
            // Since the Help method doesn't have any external dependencies,
            // we just need to make sure it doesn't throw any exceptions.

            // Act & Assert
            var exception = Record.Exception(() => _controller.Help());
            Assert.Null(exception);
        }
    }
}
