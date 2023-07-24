using AutoMapper;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;

namespace ResignationAPI.Mappings
{
    public class MappingProfile : Profile
    {
        protected MappingProfile()
        {
            CreateMap<Resignation, ResignationRequestDTO>().ReverseMap();
        }
    }
}
