using AutoMapper;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace Conduit.Profiles
{
    public class FollowProfile : Profile
    {
        public FollowProfile()
        {
            CreateMap<Follow, FollowResponseDto>();
            CreateMap<User, FollowResponseDto>();
        }
    }
}