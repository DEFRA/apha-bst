using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
using AutoMapper;

namespace Apha.BST.Application.Mappings
{
    public class EntityMapper : Profile
    {
        public EntityMapper()
        {
            // CreateMap<SourceType, DestinationType>();
            // Add your mappings here
            // For example:
            // CreateMap<WeatherForecast, WeatherForecastDto>();
            CreateMap<Persons, PersonsDTO>().ReverseMap();
            CreateMap<Site, SiteDTO>().ReverseMap();
            CreateMap<SiteTrainee, SiteTraineeDTO>().ReverseMap();
            CreateMap<TrainerTrainingDTO, Training>().ReverseMap();
            CreateMap<Trainee, PersonsDTO>().ReverseMap();
            CreateMap<TrainingDTO, Training>().ReverseMap();
            CreateMap<EditTrainingDTO, EditTraining>().ReverseMap();           
        }
    }
}
