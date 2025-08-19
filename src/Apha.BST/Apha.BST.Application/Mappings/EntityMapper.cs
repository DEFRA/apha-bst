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
            CreateMap<TraineeTrainer, TraineeDto>().ReverseMap();
            CreateMap<TrainingDto, Training>().ReverseMap();
            CreateMap<EditTrainingDto, EditTraining>().ReverseMap();           
            CreateMap<TrainerHistoryDto, TrainerHistory>().ReverseMap();
            CreateMap<TrainerTrainedDto, TrainerTrained>().ReverseMap();
            CreateMap<NewsDto, News>().ReverseMap();
            CreateMap<AddPersonDto, AddPerson>().ReverseMap();
            CreateMap<EditPersonDto, EditPerson>().ReverseMap();
            CreateMap<PersonSiteLookupDto, PersonSiteLookup>().ReverseMap();
            CreateMap<PersonLookupDto, PersonLookup>().ReverseMap();
            CreateMap<PersonDetailDto, PersonDetail>().ReverseMap();
            CreateMap<UserDto, User>().ReverseMap();
            CreateMap<UserView, UserViewDto>().ReverseMap();
            CreateMap<VlaLocDto, VlaLoc>().ReverseMap();
            CreateMap<VlaLocView, VlaLocDto>().ReverseMap();
            CreateMap<SiteInput, SiteInputDto>().ReverseMap();
            CreateMap<SiteReport, SiteReportDto>().ReverseMap();
            CreateMap<TrainerReport, TrainerReportDto>().ReverseMap();
            // Example using AutoMapper
            CreateMap<PeopleReport, PeopleReportDto>()
                .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.Name)); // or src.Location, depending on your entity

            CreateMap<TrainingReport, TrainingReportDto>().ReverseMap();
            CreateMap<AphaReport, AphaReportDto>().ReverseMap();
        }
    }
}
