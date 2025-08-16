using AutoMapper;
using Entities.Dtos;
using Entities.Models;

namespace Stajyeryotom.Infrastructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Account, AccountDto>()
              .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Section!.SectionName))
              .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentName))
              .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentId)).ReverseMap();

            CreateMap<AccountDtoForCreation, Account>();
            CreateMap<Account, AccountDtoForUpdate>()
              .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Section!.SectionName))
              .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentName))
              .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentId)).ReverseMap();
            CreateMap<Section, SectionDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Application, ApplicationDto>()
              .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Section!.SectionName))
              .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentName))
              .ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.Section!.Department!.DepartmentId)).ReverseMap();
            CreateMap<Stats, StatsDto>();
            CreateMap<Event, EventDto>().ReverseMap();
            CreateMap<Report, ReportDto>().ReverseMap();
            CreateMap<Report, ReportViewDto>();
            CreateMap<Work, WorkDto>().ReverseMap();
            CreateMap<WorkDtoForCreation, Work>();
            CreateMap<Work, WorkDtoForUpdate>()
                .ForMember(dest => dest.InternsId, opt => opt.MapFrom(src => src.Interns!.Select(i => i.Id).ToList()));
            CreateMap<WorkDtoForUpdate, Work>()
                .ForMember(dest => dest.Interns, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<MessageDtoForUpdate, Message>()
                .ForMember(dest => dest.Interns, opt => opt.Ignore())
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Message, MessageDtoForUpdate>()
                .ForMember(dest => dest.InternsId, opt => opt.MapFrom(src => src.Interns!.Select(i => i.Id).ToList()));
            CreateMap<MessageDtoForCreation, Message>();
            CreateMap<Message, MessageDto>();
        }
    }
}
