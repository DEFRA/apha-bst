using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
using FluentAssertions;
using Xunit;
using NSubstitute;
using Apha.BST.Core.Interfaces;
using Apha.BST.Application.Services;

namespace Apha.BST.Application.UnitTests.PersonServiceTest
{
    public class PersonsServiceTests : AbstractPersonServiceTest
    {
       
        private readonly IPersonsRepository _mockPersonRepository;


        [Fact]
        public async Task GetSiteNameById_ValidPersonId_ReturnsSiteName()
        {
            // Arrange
            int personId = 1;
            string expectedSiteName = "Test Site";
            MockForGetSiteNameById(expectedSiteName, personId);

            // Act
            var result = await _personsService.GetSiteNameById(personId);

            // Assert
            Assert.Equal(expectedSiteName, result);
        }

        [Fact]
        public async Task GetSiteNameById_InvalidPersonId_ReturnsNull()
        {
            // Arrange
            int personId = -1;
            string? expectedSiteName = null;
            MockForGetSiteNameById(expectedSiteName, personId);

            // Act
            var result = await _personsService.GetSiteNameById(personId);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetSiteNameById_VariousPersonIds_ReturnsExpectedSiteName(int personId)
        {
            // Arrange
            string expectedSiteName = $"Site {personId}";
            MockForGetSiteNameById(expectedSiteName, personId);

            // Act
            var result = await _personsService.GetSiteNameById(personId);

            // Assert
            Assert.Equal(expectedSiteName, result);
        }


        [Fact]
        public async Task AddPersonAsync_NewPerson_ReturnsSuccessMessage()
        {
            // Arrange
            var personDto = new AddPersonDto { Name = "Test Person", LocationId = "LOC1", PlantNo = "PLANT001" };
            string userName = "testUser";
            MockForAddPersonAsync("CREATED", personDto, userName);

            // Act
            var result = await _personsService.AddPersonAsync(personDto, userName);

            // Assert
            result.Should().Be($"{personDto.Name} saved as a person");
        }

        [Fact]
        public async Task AddPersonAsync_WithDifferentUser_ReturnsSuccessMessage()
        {
            // Arrange
            var personDto = new AddPersonDto { Name = "Test Person", LocationId = "LOC1", PlantNo = "PLANT001" };
            string userName = "differentUser";
            MockForAddPersonAsync("CREATED", personDto, userName);

            // Act
            var result = await _personsService.AddPersonAsync(personDto, userName);

            // Assert
            result.Should().Be($"{personDto.Name} saved as a person");
        }

        [Fact]
        public async Task DeletePersonAsync_PersonWithNoRecords_ReturnsSuccess()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            MockForDeletePersonAsync(personId, personName, true);

            // Act
            var result = await _personsService.DeletePersonAsync(personId);
         
            // Assert
            result.Should().Be($"{personName} from  has been deleted from the database.");
        }

        [Fact]
        public async Task DeletePersonAsync_PersonWithRecords_ReturnsWarningMessage()
        {
            // Arrange
            int personId = 1;
            string personName = "John Doe";
            MockForDeletePersonAsync(personId, personName, false);

            // Act
            var result = await _personsService.DeletePersonAsync(personId);

            // Assert
            result.Should().Be($"{personName} has training records. Delete them first if you wish to remove the person.");
        }

        [Fact]
        public async Task GetAllPersonByNameAsync_ShouldReturnPersonDetails()
        {
            // Arrange
            int personId = 1;
            MockForGetPersonByName(personId);

            // Act
            var result = await _personsService.GetAllPersonByNameAsync(personId);

            // Assert
            result.Should().NotBeNull();
            result.Should().ContainSingle();
            result.First().Person.Should().Be("John Doe");
            result.First().PlantNo.Should().Be("PLANT001");
        }

        [Fact]
        public async Task GetPersonsForDropdownAsync_SuccessfulRetrieval_ReturnsPersonDropDownDtos()
        {
            // Arrange
            var persons = new List<PersonLookup>
            {
                new PersonLookup { PersonID = 1, Person = "John Doe" },
                new PersonLookup { PersonID = 2, Person = "Jane Smith" }
            };
            MockForGetPersonsDropdown(persons);

            // Act
            var result = await _personsService.GetPersonsForDropdownAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Person.Should().Be("John Doe");
            result.Last().Person.Should().Be("Jane Smith");
        }

        [Fact]
        public async Task GetPersonsForDropdownAsync_EmptyResult_ReturnsEmptyList()
        {
            // Arrange
            MockForGetPersonsDropdown();

            // Act
            var result = await _personsService.GetPersonsForDropdownAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPersonsForDropdownAsync_ExceptionThrown_PropagatesException()
        {
            // Arrange
            MockForGetPersonsDropdownException();

            // Act & Assert
            await FluentActions
                .Invoking(async () => await _personsService.GetPersonsForDropdownAsync())
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Database error");
        }

        [Fact]
        public async Task UpdatePersonAsync_SuccessfulUpdate_ReturnsSuccessMessage()
        {
            // Arrange
            var personDto = new EditPersonDto
            {
                PersonID = 1,
                Person = "John Smith",
                Name = "VLA1"

            };
            MockForUpdatePersonAsync("SUCCESS", personDto);

            // Act
            var result = await _personsService.UpdatePersonAsync(personDto);

            // Assert
            result.Should().Be($"{personDto.Person} updated with new values");
        }

        [Fact]
        public async Task UpdatePersonAsync_FailedUpdate_ReturnsFailureMessage()
        {
            // Arrange
            var personDto = new EditPersonDto
            {
                PersonID = 1,
                Person = "John Smith",
                Name = "VLA1"
            };
            MockForUpdatePersonAsync("FAIL: Database error", personDto);

            // Act
            var result = await _personsService.UpdatePersonAsync(personDto);

            // Assert
            result.Should().NotBe($"Update Successful");
        }

        [Fact]
        public async Task GetAllSitesAsync_ReturnsSuccessfully()
        {
            // Arrange
            var sites = new List<PersonSiteLookup>
            {
                new PersonSiteLookup {  Name = "Site 1", PlantNo = "PLANT001" },
                new PersonSiteLookup {  Name = "Site 2", PlantNo = "PLANT001" }
            };
            MockForGetAllSitesAsync("PLANT001", sites);

            // Act
            var result = await _personsService.GetAllSitesAsync("PLANT001");

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be("Site 1");
            result.Last().Name.Should().Be("Site 2");
            result.Should().AllSatisfy(site => site.PlantNo.Should().Be("PLANT001"));
        }

        [Fact]
        public async Task GetAllSitesAsync_ReturnsEmptyList_WhenNoSitesFound()
        {
            // Arrange
            MockForGetAllSitesAsync("PLANT002");

            // Act
            var result = await _personsService.GetAllSitesAsync("PLANT002");

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllSitesAsync_ThrowsException_WhenRepositoryThrows()
        {
            // Arrange
            MockForGetAllSitesAsyncException("PLANT001");

            // Act & Assert
            await FluentActions
                .Invoking(async () => await _personsService.GetAllSitesAsync("PLANT001"))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Failed to retrieve sites");
        }

        [Fact]
        public async Task GetAllSitesAsync_CallsMapperWithRepositoryResult()
        {
            // Arrange
            var sites = new List<PersonSiteLookup>
            {
                new PersonSiteLookup { Name = "Site 1", PlantNo = "PLANT001" }
            };
            
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<AutoMapper.IMapper>();

            mockRepo.GetAllSitesAsync("PLANT001").Returns(sites.AsEnumerable());
            _personsService = new PersonsService(mockRepo, mockMapper);

            // Act
            await _personsService.GetAllSitesAsync("PLANT001");

            // Assert
            mockMapper.Received(1).Map<IEnumerable<PersonSiteLookupDto>>(Arg.Is<IEnumerable<PersonSiteLookup>>(s => 
                s.First().Name == "Site 1" && 
                s.First().PlantNo == "PLANT001"));
        }

        [Fact]
        public async Task GetSiteByIdAsync_ExistingPersonId_ReturnsSite()
        {
            // Arrange
            int personId = 1;
            string expectedSite = "Test Site";
            MockForGetSiteById(personId, expectedSite);

            // Act
            var result = await _personsService.GetSiteByIdAsync(personId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedSite);
        }

        [Fact]
        public async Task GetSiteByIdAsync_NonExistentPersonId_ReturnsNull()
        {
            // Arrange
            int nonExistentPersonId = 999;
            MockForGetSiteById(nonExistentPersonId, null);

            // Act
            var result = await _personsService.GetSiteByIdAsync(nonExistentPersonId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSiteByIdAsync_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int personId = 1;
            MockForGetSiteByIdException(personId);

            // Act & Assert
            await FluentActions
                .Invoking(async () => await _personsService.GetSiteByIdAsync(personId))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Failed to retrieve site information");
        }

        [Fact]
        public async Task GetPersonNameByIdAsync_ExistingPersonId_ReturnsName()
        {
            // Arrange
            int personId = 1;
            string expectedName = "John Doe";
            MockForGetPersonNameById(personId, expectedName);

            // Act
            var result = await _personsService.GetPersonNameByIdAsync(personId);

            // Assert
            result.Should().Be(expectedName);
        }

        [Fact]
        public async Task GetPersonNameByIdAsync_NonExistentPersonId_ReturnsNull()
        {
            // Arrange
            int personId = 999;
            MockForGetPersonNameById(personId, null);

            // Act
            var result = await _personsService.GetPersonNameByIdAsync(personId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetPersonNameByIdAsync_RepositoryThrowsException_ThrowsException()
        {
            // Arrange
            int personId = 1;
            MockForGetPersonNameByIdException(personId);

            // Act & Assert
            await FluentActions
                .Invoking(async () => await _personsService.GetPersonNameByIdAsync(personId))
                .Should()
                .ThrowAsync<Exception>()
                .WithMessage("Failed to retrieve person name");
        }

        // In your test class constructor or setup method, initialize _mockPersonRepository and _personsService
        public PersonsServiceTests()
        {
            _mockPersonRepository = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<AutoMapper.IMapper>();
            _personsService = new PersonsService(_mockPersonRepository, mockMapper);
        }
    }
}