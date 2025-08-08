using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
using Apha.BST.Core;
using Apha.BST.Core.Entities;
using Apha.BST.Core.Interfaces;
using Apha.BST.DataAccess.Data;
using Apha.BST.DataAccess.Repositories;
using AutoMapper;
using NSubstitute;

namespace Apha.BST.Application.UnitTests.Services
{
    public class AbstractSiteServiceTest
    {
        protected ISiteService _siteService;
        protected IMapper _mapper;

        public AbstractSiteServiceTest()
        {
            _siteService = null!;
            _mapper = null!;
        }


        public void MockForGetSites(string plantNo)
        {
            var sites = new List<Site>
        {
            new Site { PlantNo = "PLANT001", Name = "Site 1" },
            new Site { PlantNo = "PLANT002", Name = "Site 1" }
        };

            // Filter sites based on the provided plant number
            var filteredSites = sites.Where(s => s.PlantNo == plantNo).ToList();

            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetAllSitesAsync(plantNo).Returns(Task.FromResult(filteredSites.AsEnumerable()));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Site, SiteDto>()
                    .ForMember(dest => dest.PlantNo, opt => opt.MapFrom(src => src.PlantNo))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            });

            _mapper = config.CreateMapper();
            _siteService = new SiteService(mockRepo, _mapper);
        }
        public void MockForAddSiteAsync(string returnValue, SiteDto inputDto, string userName)
        {
            var mockRepo = Substitute.For<ISiteRepository>();
            var mockMapper = Substitute.For<IMapper>();

            var site = new Site { Name = inputDto.Name, PlantNo = inputDto.PlantNo };
            mockMapper.Map<Site>(inputDto).Returns(site);
            mockRepo.AddSiteAsync(site, userName).Returns(returnValue);
            _mapper = mockMapper;
            _siteService = new SiteService(mockRepo, mockMapper);
        }

        public void MockForGetSiteTraineesAsync(string plantNo)
        {
            // Sample data for trainees
            var trainees = new List<SiteTrainee>
        {
            new SiteTrainee { PersonId = 1, Person = "John Doe", Cattle = true, SheepAndGoat = false },
            new SiteTrainee { PersonId = 2, Person = "Jane Smith", Cattle = false, SheepAndGoat = true }
        };

            // Mock repository
            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetSiteTraineesAsync(plantNo).Returns(plantNo == "PLANT001" ? trainees : new List<SiteTrainee>());

            // AutoMapper configuration for SiteTrainee -> SiteTraineeDto mapping
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SiteTrainee, SiteTraineeDto>()
                    .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
                    .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person))
                    .ForMember(dest => dest.Cattle, opt => opt.MapFrom(src => src.Cattle))
                    .ForMember(dest => dest.SheepAndGoat, opt => opt.MapFrom(src => src.SheepAndGoat));
            });

            _mapper = config.CreateMapper();
            _siteService = new SiteService(mockRepo, _mapper);
        }

        public void MockforDeleteTraineeAsync(int personId, string? personName, bool deleteSuccess)
        {
            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetPersonNameByIdAsync(personId).Returns(personName);
            mockRepo.DeleteTraineeAsync(personId).Returns(deleteSuccess);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Site, SiteDto>()
                   .ForMember(dest => dest.PlantNo, opt => opt.MapFrom(src => src.PlantNo))
                   .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            });

            _mapper = config.CreateMapper();
            _siteService = new SiteService(mockRepo, _mapper);
        }
    }
}
