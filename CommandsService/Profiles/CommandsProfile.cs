using AutoMapper;
using CommandsService.DTOs;
using CommandsService.Models;

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
            CreateMap<PlatformPublishedDTO,Platform>().ForMember(destination => destination.ExternalId, opt => opt.MapFrom(src => src.Id));
        }
    }
}