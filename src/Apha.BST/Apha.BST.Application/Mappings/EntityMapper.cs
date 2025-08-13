using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
using AutoMapper;

namespace Apha.BST.Application.Mappings
{
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            CreateMap<Persons, PersonsDto>().ReverseMap();
            CreateMap<Site, SiteDto>().ReverseMap();
            CreateMap<SiteTrainee, SiteTraineeDto>().ReverseMap();
            CreateMap<TrainerTrainingDto, TrainerTraining>().ReverseMap();
            CreateMap<TraineeTrainer, PersonsDto>().ReverseMap();
            CreateMap<TrainingDto, Training>().ReverseMap();
            CreateMap<EditTrainingDto, EditTraining>().ReverseMap();           
            CreateMap<TrainerHistoryDto, TrainerHistory>().ReverseMap();
            CreateMap<TrainerTrainedDto, TrainerTrained>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserView, UserViewDto>().ReverseMap();
            CreateMap<VlaLocDto, VlaLoc>().ReverseMap();
            CreateMap<VlaLocView, VlaLocDto>().ReverseMap();
            CreateMap<NewsDto, News>().ReverseMap();

        }
    }
}
