using System;
using asp_dotnet_realworld_conduit.DTOs.Requests;
using asp_dotnet_realworld_conduit.DTOs.Responses;
using asp_dotnet_realworld_conduit.Models;
using AutoMapper;

namespace asp_dotnet_realworld_conduit.Profiles
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<User, UserLoginRequestDto>();
            CreateMap<User, UserLoginInfoDto>();
        }
    }
}
