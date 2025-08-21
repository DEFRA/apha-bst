using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.UnitTests.RoleMappingServiceTest
{
    public abstract class AbstractRoleMappingServiceTest
    {
        protected IRoleMappingService RoleMappingService;

        protected AbstractRoleMappingServiceTest()
        {
            RoleMappingService = new RoleMappingService();
        }

        public async Task MockForAssertGetRoleName(byte roleId, string expectedRoleName)
        {
            var result = await RoleMappingService.GetRoleName(roleId);
            Assert.Equal(expectedRoleName, result);
        }

        public void MockForAssertGetUserLevels(List<SelectListItem> expectedItems)
        {
            var result = RoleMappingService.GetUserLevels();
            Assert.Equal(expectedItems.Count, result.Count);
            foreach (var expectedItem in expectedItems)
            {
                Assert.Contains(result, item => item.Value == expectedItem.Value && item.Text == expectedItem.Text);
            }
        }
        public List<SelectListItem> GetUserLevels()
        {
            return RoleMappingService.GetUserLevels();
        }

        public void MockForAssertUserLevels(Action<List<SelectListItem>> assertion)
        {
            var result = GetUserLevels();
            assertion(result);
        }
    }

}
