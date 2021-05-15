using System;
using AutoMapper;
using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace Conduit.Profiles
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<Article, ArticlesResponseDto>();
            CreateMap<User, AuthorResponseDto>();
            CreateMap<ArticleCreateRequestDto, Article>();
            CreateMap<ArticleUpdateRequestDto, Article>();
        }
    }
}
