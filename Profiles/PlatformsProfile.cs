using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService.Profiles
{
    public class PlatformsProfile : Profile
    {
        public PlatformsProfile()
        {
            // Source -> target!! it`s not a source <-> target!!!
            CreateMap<Platform, PlatformReadDto>();
            CreateMap<PlatformCreateDto, Platform>(); //it will map fields from PCreDto to Platform
            
        }
    }
}