using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;
using AutoMapper;

namespace Conduit.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UsersResponseDto>();
            CreateMap<UserUpdateRequestDto, User>();
        }
    }
}
