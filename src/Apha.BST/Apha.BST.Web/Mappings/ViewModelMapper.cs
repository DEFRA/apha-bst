using Apha.BST.Application.DTOs;
using Apha.BST.Core.Entities;
using Apha.BST.Web.Models;
using AutoMapper;

namespace Apha.BST.Web.Mappings
{
    public class ViewModelMapper : Profile
    {
        public ViewModelMapper() 
        {
            CreateMap<SiteViewModel, SiteDto>().ReverseMap();
            CreateMap<SiteTraineeViewModel, SiteTraineeDto>().ReverseMap();
            CreateMap<AddTrainingViewModel, TrainingDto>().ReverseMap();
            CreateMap<TrainerTraining, TrainerTrainingDto>().ReverseMap();
            CreateMap<TrainerTrainingDto, TrainingViewModel>().ReverseMap();            
            CreateMap<PersonsDto, TraineeViewModel>().ReverseMap();
            CreateMap<PersonsDto, TrainerTrainedModel>().ReverseMap();
            CreateMap<EditTrainingDto, EditTrainingViewModel>().ReverseMap();
            CreateMap<TrainerHistoryDto, TrainingHistoryModel>().ReverseMap();
            CreateMap<TrainerTrainedDto, TrainerTrainedViewModel>().ReverseMap();
        }
    }
}
