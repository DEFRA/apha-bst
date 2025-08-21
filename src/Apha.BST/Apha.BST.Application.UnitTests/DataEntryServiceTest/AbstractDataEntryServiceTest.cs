using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apha.BST.Application.UnitTests.DataEntryServiceTest
{
    public abstract class AbstractDataEntryServiceTest
    {
        protected IDataEntryService? Service;
        protected IDataEntryRepository? MockRepository;




        public void MockCanEditPage(string action, bool returnValue)
        {
            var mockRepo = Substitute.For<IDataEntryRepository>();

            mockRepo.CanEditPage(action).Returns(Task.FromResult(returnValue));

            Service = new DataEntryService(mockRepo);
            MockRepository = mockRepo;
        }

        public void SetupMockCanEditPageThrowsException(string action, Exception exception)
        {
            var mockRepo = Substitute.For<IDataEntryRepository>();

            mockRepo.CanEditPage(action).Throws(exception);
            Service = new DataEntryService(mockRepo);
            MockRepository = mockRepo;
        }

        public async Task AssertCanEditPageCalled(string action, int times = 1)
        {

            await MockRepository!.Received(times).CanEditPage(action);
        }

    }
}
