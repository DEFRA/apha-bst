using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.UnitTests.Helpers;
using Moq;

namespace Apha.BST.DataAccess.UnitTests.ReportRepositoryTest
{
    public class ReportRepositoryTests
    {
        [Fact]
        public async Task GetAphaReportsAsync_ReturnsReports()
        {
            var reports = new TestAsyncEnumerable<AphaReport>(new[]
            {
                new AphaReport { ID = "1", Location = "Loc1", APHA = "A1" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractReportRepositoryTest(mockContext.Object, aphaReports: reports);

            var result = await repo.GetAphaReportsAsync();

            Assert.Single(result);
            Assert.Equal("Loc1", result[0].Location);
        }

        [Fact]
        public async Task GetPeopleReportsAsync_ReturnsReports()
        {
            var reports = new TestAsyncEnumerable<PeopleReport>(new[]
            {
                new PeopleReport { PersonID = 1, Person = "John", Name = "N1" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractReportRepositoryTest(mockContext.Object, peopleReports: reports);

            var result = await repo.GetPeopleReportsAsync();

            Assert.Single(result);
            Assert.Equal("John", result[0].Person);
        }

        [Fact]
        public async Task GetSiteReportsAsync_ReturnsReports()
        {
            var reports = new TestAsyncEnumerable<SiteReport>(new[]
            {
                new SiteReport { PlantNo = "P1", Name = "Site1" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractReportRepositoryTest(mockContext.Object, siteReports: reports);

            var result = await repo.GetSiteReportsAsync();

            Assert.Single(result);
            Assert.Equal("Site1", result[0].Name);
        }

        [Fact]
        public async Task GetTrainerReportsAsync_ReturnsReports()
        {
            var reports = new TestAsyncEnumerable<TrainerReport>(new[]
            {
                new TrainerReport { ID = 1 }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractReportRepositoryTest(mockContext.Object, trainerReports: reports);

            var result = await repo.GetTrainerReportsAsync();

            Assert.Single(result);
            Assert.Equal(1, result[0].ID);
        }

        [Fact]
        public async Task GetTrainingReportsAsync_ReturnsReports()
        {
            var reports = new TestAsyncEnumerable<TrainingReport>(new[]
            {
                new TrainingReport { Trainer = "T1", Trainee = "TR1", VLA = "V1" }
            });
            var mockContext = new Mock<BstContext>();
            var repo = new AbstractReportRepositoryTest(mockContext.Object, trainingReports: reports);

            var result = await repo.GetTrainingReportsAsync();

            Assert.Single(result);
            Assert.Equal("T1", result[0].Trainer);
        }
    }
}
