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

        public void MockForAddTraining(int returnCode, string personName = "John Doe", string trainerName = "Jane Smith", DateTime? date = null)
        {
            var trainingDto = new TrainingDTO
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = "Test Type",
                TrainingDateTime = date ?? DateTime.Now
            };

            var training = new Training
            {
                PersonId = 1,
                TrainerId = 2,
                TrainingType = trainingDto.TrainingType,
                TrainingDateTime = trainingDto.TrainingDateTime
            };

            var mockRepo = Substitute.For<ITrainingRepository>();

            mockRepo.AddTrainingAsync(Arg.Any<Training>())
                .Returns(Task.FromResult(new AddTrainingResult { ReturnCode = (byte)returnCode }));
            mockRepo.GetPersonByIdAsync(1)
                .Returns(Task.FromResult(new Persons { PersonId = 1, Person = personName }));
            mockRepo.GetPersonByIdAsync(2)
                .Returns(Task.FromResult(new Persons { PersonId = 2, Person = trainerName }));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrainingDTO, Training>();
            });

            _mapper = config.CreateMapper();
            _trainingService = new TrainingService(mockRepo, _mapper);
        }

        public void MockForAddTrainingWithNullNames(int? personId, int? trainerId)
        {
            var trainingDto = new TrainingDTO
            {
                PersonId = personId ?? 0,
                TrainerId = trainerId ?? 0,
                TrainingType = "Test Type",
                TrainingDateTime = DateTime.Now
            };

            var training = new Training
            {
                PersonId = trainingDto.PersonId,
                TrainerId = trainingDto.TrainerId,
                TrainingType = trainingDto.TrainingType,
                TrainingDateTime = trainingDto.TrainingDateTime
            };

            var mockRepo = Substitute.For<ITrainingRepository>();

            mockRepo.AddTrainingAsync(Arg.Any<Training>())
                .Returns(Task.FromResult(new AddTrainingResult { ReturnCode = 0 }));
            mockRepo.GetPersonByIdAsync(Arg.Any<int>())
                .Returns(Task.FromResult<Persons?>(null));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TrainingDTO, Training>();
            });

            _mapper = config.CreateMapper();
            _trainingService = new TrainingService(mockRepo, _mapper);
        }
        public void MockForGetAllTrainings(IEnumerable<TrainerTraining> trainings, IEnumerable<TrainerTrainingDTO> mappedTrainings)
        {
            var mockRepo = Substitute.For<ITrainingRepository>();
            mockRepo.GetAllTrainingsAsync().Returns(trainings);

            var mockMapper = Substitute.For<IMapper>();
            mockMapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings).Returns(mappedTrainings);

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }
      
        public void MockForGetTrainingByTrainee(string traineeId, IEnumerable<TrainerTraining> trainings, IEnumerable<TrainerTrainingDTO> mappedTrainings)
        {
            var idToUse = (traineeId == "All" || !int.TryParse(traineeId, out _)) ? "0" : traineeId;

            var mockRepo = Substitute.For<ITrainingRepository>();
            mockRepo.GetTrainingByTraineeAsync(idToUse).Returns(Task.FromResult(trainings));

            var mockMapper = Substitute.For<IMapper>();
            mockMapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings).Returns(mappedTrainings);

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
                    mockMapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings)
                              .Throws(new AutoMapperMappingException("Mapping error"));
                }
                else
                {
                    mockMapper.Map<IEnumerable<TrainerTrainingDTO>>(trainings)
                              .Returns(new List<TrainerTrainingDTO> { new TrainerTrainingDTO() });
                }
            }

            _trainingService = new TrainingService(mockRepo, mockMapper);
        }

    }
}
