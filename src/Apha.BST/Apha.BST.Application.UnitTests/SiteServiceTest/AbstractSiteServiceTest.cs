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

        protected AbstractSiteServiceTest()
        {
           
        }

        public void MockforGetSites()
        {
            var sites = new List<Site> {
                new Site { PlantNo = "PLANT001", Name = "Site 1" },
             };
            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetAllSitesAsync("PLANT001").Returns(Task.FromResult(sites.AsEnumerable()));
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Site, SiteDto>()
                   .ForMember(dest => dest.PlantNo, opt => opt.MapFrom(src => src.PlantNo))
                   .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            });

            _mapper = config.CreateMapper();
            // _siteRepository = new SiteRepository(_dbContext);
            _siteService = new SiteService(mockRepo, _mapper);
        }


        public void MockForAddSiteAsync(string returnValue, SiteDto inputDto)
        {
            var mockRepo = Substitute.For<ISiteRepository>();
            var mockMapper = Substitute.For<IMapper>();

            var site = new Site { Name = inputDto.Name, PlantNo = inputDto.PlantNo };
            mockMapper.Map<Site>(inputDto).Returns(site);
            mockRepo.AddSiteAsync(site).Returns(returnValue);

            _mapper = mockMapper;
            _siteService = new SiteService(mockRepo, mockMapper);
        }

        public void MockForGetSiteTrainees(string plantNo)
        {
            var trainees = plantNo switch
            {
                "PLANT001" => new List<SiteTrainee>
            {
                 new SiteTrainee { PersonId = 1, Person = "John Doe", Cattle = true, Sheep = false, Goats = true },
                 new SiteTrainee { PersonId = 2, Person = "Jane Smith", Cattle = false, Sheep = true, Goats = false }
            },
                _ => new List<SiteTrainee>()
            };

            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetSiteTraineesAsync(plantNo).Returns(Task.FromResult(trainees));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SiteTrainee, SiteTraineeDto>()
                   .ForMember(dest => dest.PersonId, opt => opt.MapFrom(src => src.PersonId))
                   .ForMember(dest => dest.Person, opt => opt.MapFrom(src => src.Person))
                   .ForMember(dest => dest.Cattle, opt => opt.MapFrom(src => src.Cattle))
                   .ForMember(dest => dest.Sheep, opt => opt.MapFrom(src => src.Sheep))
                   .ForMember(dest => dest.Goats, opt => opt.MapFrom(src => src.Goats));
            });

            _mapper = config.CreateMapper();
            _siteService = new SiteService(mockRepo, _mapper);
        }
    }
}
