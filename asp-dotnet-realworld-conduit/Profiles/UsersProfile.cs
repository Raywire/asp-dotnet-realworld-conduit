using System;
using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;
using AutoMapper;

namespace asp_dotnet_realworld_conduit.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<Users, UsersResponseDto>();
            CreateMap<UserUpdateRequestDto, Users>();
        }
    }
}
