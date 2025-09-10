using Apha.BST.Application.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.UnitTests.StaticDropdownServiceTest
{
    public abstract class AbstractStaticDropdownServiceTest
    {
        protected readonly IStaticDropdownService _staticDropdownService;

        protected AbstractStaticDropdownServiceTest()
        {
            _staticDropdownService = Substitute.For<IStaticDropdownService>();
        }

        protected void VerifyGetTrainingTypes(List<SelectListItem> expectedResult)
        {
            // Act
            var result = _staticDropdownService.GetTrainingTypes();

            // Assert
            Assert.Equal(expectedResult, result);
        }

        protected void VerifyGetTrainingAnimal(List<SelectListItem> expectedResult)
        {
            // Act
            var result = _staticDropdownService.GetTrainingAnimal();

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
