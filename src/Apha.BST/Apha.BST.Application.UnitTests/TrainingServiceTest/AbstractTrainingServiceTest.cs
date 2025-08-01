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
using AutoMapper;
using FluentAssertions.Execution;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace Apha.BST.Application.UnitTests.TrainingServiceTest
{
    public abstract class AbstractTrainingServiceTest
    {
        protected ITrainingService? _trainingService;
        protected IMapper? _mapper;
        protected SiteService? SiteServiceInstance;

        public void MockForAddTrainingAsync(TrainingDto trainingDto,Training trainingEntity, string repoResponse,Persons? trainee,Persons? trainer)
        {
            var mockRepo = Substitute.For<ITrainingRepository>();
            var mockMapper = Substitute.For<IMapper>();

            mockMapper.Map<Training>(trainingDto).Returns(trainingEntity);
            mockRepo.AddTrainingAsync(trainingEntity).Returns(repoResponse);
            mockRepo.GetPersonByIdAsync(trainingDto.PersonId).Returns(Task.FromResult(trainee));
            mockRepo.GetPersonByIdAsync(trainingDto.TrainerId).Returns(Task.FromResult(trainer));

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }
        public void MockForGetAllTrainings(IEnumerable<TrainerTraining> trainings, IEnumerable<TrainerTrainingDto> mappedTrainings)
        {
            var mockRepo = Substitute.For<ITrainingRepository>();
            mockRepo.GetAllTrainingsAsync().Returns(trainings);

            var mockMapper = Substitute.For<IMapper>();
            mockMapper.Map<IEnumerable<TrainerTrainingDto>>(trainings).Returns(mappedTrainings);

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }
      
        public void MockForGetTrainingByTrainee(string traineeId, IEnumerable<TrainerTraining> trainings, IEnumerable<TrainerTrainingDto> mappedTrainings)
        {
            var idToUse = (traineeId == "All" || !int.TryParse(traineeId, out _)) ? "0" : traineeId;

            var mockRepo = Substitute.For<ITrainingRepository>();
            mockRepo.GetTrainingByTraineeAsync(idToUse).Returns(Task.FromResult(trainings));

            var mockMapper = Substitute.For<IMapper>();
            mockMapper.Map<IEnumerable<TrainerTrainingDto>>(trainings).Returns(mappedTrainings);

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }

        public void MockForTrainingByTrainee_Exception(string traineeId, bool repoThrows, bool mapperThrows)
        {
            var mockRepo = Substitute.For<ITrainingRepository>();
            var mockMapper = Substitute.For<IMapper>();

            if (repoThrows)
            {
                mockRepo.GetTrainingByTraineeAsync(traineeId).ThrowsAsync(new Exception("Database error"));
            }
            else
            {
                var trainings = new List<TrainerTraining> { new TrainerTraining() };
                mockRepo.GetTrainingByTraineeAsync(traineeId).Returns(Task.FromResult<IEnumerable<TrainerTraining>>(trainings));

                if (mapperThrows)
                {
                    mockMapper.Map<IEnumerable<TrainerTrainingDto>>(trainings)
                              .Throws(new AutoMapperMappingException("Mapping error"));
                }
                else
                {
                    mockMapper.Map<IEnumerable<TrainerTrainingDto>>(trainings)
                              .Returns(new List<TrainerTrainingDto> { new TrainerTrainingDto() });
                }
            }

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }
        public void MockForGetTrainerHistory(int personId, string animalType, IEnumerable<TrainerHistory> history, IEnumerable<TrainerHistoryDto> mappedDtos)
        {
            var mockRepo = Substitute.For<ITrainingRepository>();
            mockRepo.GetTrainerHistoryAsync(personId, animalType).Returns(Task.FromResult(history));

            var mockMapper = Substitute.For<IMapper>();
            mockMapper.Map<IEnumerable<TrainerHistoryDto>>(history).Returns(mappedDtos);

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }


    }
}
