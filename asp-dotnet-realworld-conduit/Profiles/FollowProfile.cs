using AutoMapper;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace asp_dotnet_realworld_conduit.Profiles
{
    public class FollowProfile : Profile
    {
        public FollowProfile()
        {
            CreateMap<Follow, FollowingResponseDto>();
            CreateMap<Follow, FollowerResponseDto>();
        }
    }
}