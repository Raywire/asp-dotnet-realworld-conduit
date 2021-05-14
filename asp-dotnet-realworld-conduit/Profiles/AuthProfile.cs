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
            CreateMap<Users, UserLoginRequestDto>();
            CreateMap<Users, UserLoginInfoDto>();
            CreateMap<UserRegisterRequestDto, Users>();
        }
    }
}
