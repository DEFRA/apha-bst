using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Application.DTOs;
using Apha.BST.Application.Interfaces;
using Apha.BST.Application.Services;
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
        //protected readonly SiteRepository _siteRepository;
        //protected readonly BSTContext _dbContext;
        protected IMapper _mapper;

        protected AbstractSiteServiceTest()
        {
            //_dbContext = SqliteTestContextMother.CreateContext();

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<Site, SiteDTO>()
            //       .ForMember(dest => dest.PlantNo, opt => opt.MapFrom(src => src.PlantNo))
            //       .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            //});

            //_mapper = config.CreateMapper();
            //_siteRepository = new SiteRepository(_dbContext);
            //_siteService = new SiteService(_siteRepository, _mapper);
            //SeedData();
        }
    //    private void SeedData()
    //    {
    //        var sites = new List<Site>
    //{
    //    new Site { PlantNo = "PLANT001", Name = "Site 1" },
    //    new Site { PlantNo = "PLANT002", Name = "Site 2" },
    //    new Site { PlantNo = "PLANT003", Name = "Site 3" }
    //};
    //        _dbContext.Sites.AddRange(sites);
    //        _dbContext.SaveChanges();
    //    }

        public void MockforGetSites()
        {
            var sites = new List<Site> {
                new Site { PlantNo = "PLANT001", Name = "Site 1" },
             };
            var mockRepo = Substitute.For<ISiteRepository>();
            mockRepo.GetAllSitesAsync("PLANT001").Returns(Task.FromResult(sites.AsEnumerable()));
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Site, SiteDTO>()
                   .ForMember(dest => dest.PlantNo, opt => opt.MapFrom(src => src.PlantNo))
                   .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));
            });

            _mapper = config.CreateMapper();
            // _siteRepository = new SiteRepository(_dbContext);
            _siteService = new SiteService(mockRepo, _mapper);
        }
    }
}
