using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;
using PlatformService;


namespace CommandsService.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            // source => Target 
            CreateMap<Platform, PlatformReadDTO>();
            CreateMap<Command, CommandReadDTO>();
            CreateMap<CommandCreateDTO, Command>();
            CreateMap<PlatformPublishedDTO, Platform>()
            .ForMember(destination => destination.ExternalId, opt => opt.MapFrom(src => src.Id));
            CreateMap<GrpcPlatfromModel, Platform>()
            .ForMember(destination => destination.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
            .ForMember(destination => destination.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(destination => destination.Commands, opt => opt.Ignore());

        }
    }
}