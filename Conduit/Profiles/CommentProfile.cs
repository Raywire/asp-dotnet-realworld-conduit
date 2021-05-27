using AutoMapper;
using Conduit.DTOs.Requests;
using Conduit.DTOs.Responses;
using Conduit.Models;

namespace Conduit.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, CommentsResponseDto>();
            CreateMap<CommentCreateRequestDto, Comment>();
        }
    }
}
