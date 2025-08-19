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
using AutoMapper;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Apha.BST.Application.UnitTests.ReportServiceTest
{
    public abstract class AbstractReportServiceTest
    {
        protected IReportService? _reportService;
        protected IReportRepository? _reportRepository;
        protected IMapper? _mapper;
        protected IConfiguration? _configuration;

        public void MockGenerateExcelReportAsync(List<SiteReport> sites,List<PeopleReport> people,List<TrainerReport> trainers,List<TrainingReport> training,List<AphaReport> apha,string? templatePath = null)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            // Repository returns entities
            mockRepo.GetSiteReportsAsync().Returns(sites);
            mockRepo.GetPeopleReportsAsync().Returns(people);
            mockRepo.GetTrainerReportsAsync().Returns(trainers);
            mockRepo.GetTrainingReportsAsync().Returns(training);
            mockRepo.GetAphaReportsAsync().Returns(apha);

            // Mapper returns DTOs
            mockMapper.Map<List<SiteReportDto>>(sites).Returns(sites.Select(s => new SiteReportDto { Name = s.Name }).ToList());
            mockMapper.Map<List<PeopleReportDto>>(people).Returns(people.Select(p => new PeopleReportDto { Person = p.Person }).ToList());
            mockMapper.Map<List<TrainerReportDto>>(trainers).Returns(trainers.Select(t => new TrainerReportDto { Trainer = t.Trainer }).ToList());
            mockMapper.Map<List<TrainingReportDto>>(training).Returns(training.Select(t => new TrainingReportDto { Trainer = t.Trainer }).ToList());
            mockMapper.Map<List<AphaReportDto>>(apha).Returns(apha.Select(a => new AphaReportDto { Location = a.Location }).ToList());

            if (templatePath != null)
                mockConfig["ReportSettings:TemplateFilePath"].Returns(templatePath);
            else
                mockConfig["ReportSettings:TemplateFilePath"].Returns((string?)null);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }


        public void MockGetSiteReportsAsync(List<SiteReport> entities, List<SiteReportDto> dtos)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            mockRepo.GetSiteReportsAsync().Returns(entities);
            mockMapper.Map<List<SiteReportDto>>(entities).Returns(dtos);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }
        public void MockGetPeopleReportsAsync(List<PeopleReport> entities, List<PeopleReportDto> dtos)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            mockRepo.GetPeopleReportsAsync().Returns(entities);
            mockMapper.Map<List<PeopleReportDto>>(entities).Returns(dtos);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }

        public void MockGetTrainerReportsAsync(List<TrainerReport> entities, List<TrainerReportDto> dtos)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            mockRepo.GetTrainerReportsAsync().Returns(entities);
            mockMapper.Map<List<TrainerReportDto>>(entities).Returns(dtos);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }

        public void MockGetTrainingReportsAsync(List<TrainingReport> entities, List<TrainingReportDto> dtos)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            mockRepo.GetTrainingReportsAsync().Returns(entities);
            mockMapper.Map<List<TrainingReportDto>>(entities).Returns(dtos);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }

        public void MockGetAphaReportsAsync(List<AphaReport> entities, List<AphaReportDto> dtos)
        {
            var mockRepo = Substitute.For<IReportRepository>();
            var mockMapper = Substitute.For<IMapper>();
            var mockConfig = Substitute.For<IConfiguration>();

            mockRepo.GetAphaReportsAsync().Returns(entities);
            mockMapper.Map<List<AphaReportDto>>(entities).Returns(dtos);

            _reportService = new ReportService(mockRepo, mockMapper, mockConfig);
            _reportRepository = mockRepo;
            _mapper = mockMapper;
            _configuration = mockConfig;
        }
    }
}
