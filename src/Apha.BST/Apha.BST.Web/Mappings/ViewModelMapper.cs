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
            CreateMap<TraineeDto, TraineeTrainerViewModel>().ReverseMap();
            CreateMap<TraineeDto, TrainerTrainedModel>().ReverseMap();
            CreateMap<EditTrainingDto, EditTrainingViewModel>().ReverseMap();
            CreateMap<TrainerHistoryDto, TrainingHistoryModel>().ReverseMap();
            CreateMap<TrainerTrainedDto, TrainerTrainedViewModel>().ReverseMap();
            CreateMap<NewsDto, NewsViewModel>().ReverseMap();           
            CreateMap<PersonDetailDto, PersonViewModel>().ReverseMap();
            CreateMap<AddPersonDto, AddPersonViewModel>().ReverseMap();
            CreateMap<EditPersonDto, EditPersonViewModel>().ReverseMap();
            CreateMap<UserDto, AddUserViewModel>().ReverseMap();
            CreateMap<AddUserViewModel, UserDto>().ReverseMap();
            CreateMap<EditUserViewModel, UserDto>().ReverseMap();
            CreateMap<UserViewDto, ViewUserViewModel>().ReverseMap();           
            CreateMap<AddNewsViewModel, NewsDto>().ReverseMap();
            CreateMap<SiteInputDto, EditSiteViewModel>().ReverseMap();
            CreateMap<ReportViewModel, ReportDto>().ReverseMap();
            CreateMap<AuditLogViewModel, AuditLogDto>().ReverseMap();
            CreateMap<AuditLogsArchivedViewModel, AuditLogArchivedDto>().ReverseMap();

        }
    }
}
