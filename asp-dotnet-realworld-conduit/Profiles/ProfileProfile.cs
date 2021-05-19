using AutoMapper;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace Conduit.Profiles
{
    public class ProfileProfile : Profile
    {
        public ProfileProfile()
        {
            CreateMap<User, ProfileResponseDto>();
        }
    }
}
