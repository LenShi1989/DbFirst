using AutoMapper;
using DbFirst.Dtos;
using DbFirst.Models;

namespace DbFirst.Profiles
{
    public class UploadFileProfile : Profile
    {
        public UploadFileProfile()
        {
            CreateMap<UploadFile, UploadFileDto>();
            CreateMap<uploadFilePostDto, UploadFile>();
        }
    }
}
