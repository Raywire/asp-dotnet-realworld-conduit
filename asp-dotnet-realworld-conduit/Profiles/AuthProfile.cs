using System;
using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;
using AutoMapper;

namespace asp_dotnet_realworld_conduit.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, UserLoginRequestDto>();
            CreateMap<User, UserLoginInfoDto>();
            CreateMap<UserRegisterRequestDto, User>();
        }
    }
}
