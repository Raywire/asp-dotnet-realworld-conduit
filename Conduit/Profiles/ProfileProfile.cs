using System.Linq;
using AutoMapper;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace Conduit.Profiles
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<User, ProfileResponseDto>()
                .ForMember(dto => dto.Followers, c => c.MapFrom(c => c.Followers.Select(cs => cs.Follower)))
                .ForMember(dto => dto.Following, c => c.MapFrom(c => c.Following.Select(cs => cs.Following)));
        }
    }
}
