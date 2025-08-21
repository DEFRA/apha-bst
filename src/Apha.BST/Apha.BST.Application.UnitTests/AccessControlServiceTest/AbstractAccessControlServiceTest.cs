using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Interfaces;
using NSubstitute;


namespace Apha.BST.Application.UnitTests.AccessControlServiceTest
{
    public abstract class AbstractAccessControlServiceTest
    {
        protected IAccessControlRepository AccessControlRepository;
        protected IAccessControlService AccessControlService;

        protected AbstractAccessControlServiceTest()
        {
            AccessControlRepository = Substitute.For<IAccessControlRepository>();
            AccessControlService = new AccessControlService(AccessControlRepository);
        }

        protected void MockGetRoleIdAndUsernameByEmailAsync(string email, byte? roleId, string? username)
        {
            if (roleId.HasValue && username != null)
            {
                AccessControlRepository.GetRoleIdAndUsernameByEmailAsync(email)
                    .Returns((roleId.Value, username));
            }
            else
            {
                AccessControlRepository.GetRoleIdAndUsernameByEmailAsync(email)
             .Returns(Task.FromResult<(byte? RoleId, string? Username)?>(null));
            }
        }

        protected async Task AssertGetRoleIdAndUsernameByEmailAsyncCalled(string email, int times = 1)
        {
            await AccessControlRepository.Received(times).GetRoleIdAndUsernameByEmailAsync(email);
        }
    }
}
