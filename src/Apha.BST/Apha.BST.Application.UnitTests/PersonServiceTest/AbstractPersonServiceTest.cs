using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;

namespace Apha.BST.Application.UnitTests.PersonServiceTest
{
    public class AbstractPersonServiceTest
    {
        protected IPersonsService _personsService;
        protected IMapper _mapper;
        protected IPersonsRepository _mockPersonRepo;


        public AbstractPersonServiceTest()
        {
            _personsService = null!;
            _mapper = null!;
            _mockPersonRepo = null!;
        }
        protected void MockForGetSiteNameById(string? expectedSiteName, int personId)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetSiteNameById(personId).Returns(Task.FromResult(expectedSiteName));

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForAddPersonAsync(string returnValue, AddPersonDto inputDto, string userName = "testUser")
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            var person = new AddPerson 
            { 
                Name = inputDto.Name, 
                LocationId = inputDto.LocationId, 
                PlantNo = inputDto.PlantNo 
            };
            
            mockMapper.Map<AddPerson>(inputDto).Returns(person);
            mockRepo.AddPersonAsync(person, userName).Returns(returnValue);

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForDeletePersonAsync(int personId, string? personName, bool deleteSuccess)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            mockRepo.GetPersonNameByIdAsync(personId).Returns(personName);
            mockRepo.DeletePersonAsync(personId).Returns(deleteSuccess);
            mockRepo.GetSiteByIdAsync(personId).Returns("Test Site");

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Persons, PersonsDto>();
            });

            _mapper = config.CreateMapper();
            _personsService = new PersonsService(mockRepo, _mapper);
        }
        public void MockForGetSiteNameById(int personId, string? expectedSiteName)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            mockRepo.GetSiteNameById(personId).Returns(expectedSiteName);

            var config = new MapperConfiguration(cfg =>
            {
               
            });

            _mapper = config.CreateMapper();
          
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetPersonByName(int personId)
        {
            var personDetails = new List<PersonDetail>
            {
                new PersonDetail { PersonID = personId, Person = "John Doe", PlantNo = "PLANT001", Name = "Test Site" }
            };

            var mockRepo = Substitute.For<IPersonsRepository>();
            mockRepo.GetAllPersonByNameAsync(personId).Returns(Task.FromResult(personDetails.AsEnumerable()));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonDetail, PersonDetailDto>();
            });

            _mapper = config.CreateMapper();
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetPersonsDropdown(List<PersonLookup>? persons = null)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            
            if (persons != null)
            {
                mockRepo.GetAllPersonsForDropdownAsync().Returns(Task.FromResult(persons.AsEnumerable()));
            }
            else
            {
                mockRepo.GetAllPersonsForDropdownAsync().Returns(Task.FromResult(Enumerable.Empty<PersonLookup>()));
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonLookup, PersonLookupDto>();
            });

            _mapper = config.CreateMapper();
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetPersonsDropdownException()
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            mockRepo.GetAllPersonsForDropdownAsync().Throws(new Exception("Database error"));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PersonLookup, PersonLookupDto>();
            });

            _mapper = config.CreateMapper();
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForUpdatePersonAsync(string returnValue, EditPersonDto inputDto)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            var person = new EditPerson 
            { 
                PersonID = inputDto.PersonID,
                Person = inputDto.Person,
               
                Name = inputDto.Name
            };
            
            mockMapper.Map<EditPerson>(inputDto).Returns(person);
            mockRepo.UpdatePersonAsync(person).Returns(returnValue);

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetAllSitesAsync(string plantNo, List<PersonSiteLookup>? sites = null)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (sites != null)
            {
                mockRepo.GetAllSitesAsync(plantNo).Returns(Task.FromResult(sites.AsEnumerable()));
                mockMapper.Map<IEnumerable<PersonSiteLookupDto>>(sites).Returns(sites.Select(s => new PersonSiteLookupDto
                { 
                   
                    Name = s.Name,
                    PlantNo = s.PlantNo 
                }));
            }
            else
            {
                mockRepo.GetAllSitesAsync(plantNo).Returns(Task.FromResult(Enumerable.Empty<PersonSiteLookup>()));
                mockMapper.Map<IEnumerable<PersonSiteLookupDto>>(Arg.Any<IEnumerable<PersonSiteLookup>>())
                    .Returns(Enumerable.Empty<PersonSiteLookupDto>());
            }

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetAllSitesAsyncException(string plantNo)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetAllSitesAsync(plantNo).Throws(new Exception("Failed to retrieve sites"));

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetSiteById(int personId, string? site)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (site != null)
            {
                mockRepo.GetSiteByIdAsync(personId).Returns(site);
            }
            else
            {
                mockRepo.GetSiteByIdAsync(personId).ReturnsNull();
            }

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        public void MockForGetSiteByIdException(int personId)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetSiteByIdAsync(personId).Throws(new Exception("Failed to retrieve site information"));

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

        /// <summary>
        /// Mocks the GetPersonNameById operation with a specified return value.
        /// </summary>
        /// <param name="personId">The ID of the person to look up</param>
        /// <param name="name">The name to return, or null for non-existent persons</param>
        public void MockForGetPersonNameById(int personId, string? name)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (name != null)
            {
                mockRepo.GetPersonNameByIdAsync(personId).Returns(name);
            }
            else
            {
                mockRepo.GetPersonNameByIdAsync(personId).ReturnsNull();
            }

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }

       
        public void MockForGetPersonNameByIdException(int personId)
        {
            var mockRepo = Substitute.For<IPersonsRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockRepo.GetPersonNameByIdAsync(personId)
                   .Throws(new Exception("Failed to retrieve person name"));

            _mapper = mockMapper;
            _personsService = new PersonsService(mockRepo, _mapper);
        }
    }
}