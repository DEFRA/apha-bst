using Apha.BST.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Web.UnitTests.Helpers
{
    public class CustomEmailAttributeTests
    {
        [Fact]
        public void IsValid_NullInput_ReturnsTrue()
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(null);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_NonStringInput_ReturnsFalse()
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(123);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_EmptyString_ReturnsTrue()
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(string.Empty);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_WhitespaceString_ReturnsTrue()
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(" ");

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name+tag@example.co.uk")]
        [InlineData("user-name@example.org")]
        public void IsValid_ValidEmailAddresses_ReturnsTrue(string email)
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("invalid@")]
        [InlineData("@invalid.com")]
        [InlineData("invalid@.com")]
        [InlineData("invalid@com")]
        public void IsValid_InvalidEmailAddresses_ReturnsFalse(string email)
        {
            // Arrange
            var attribute = new CustomEmailAttribute();

            // Act
            var result = attribute.IsValid(email);

            // Assert
            Assert.False(result);
        }
    }
    }
