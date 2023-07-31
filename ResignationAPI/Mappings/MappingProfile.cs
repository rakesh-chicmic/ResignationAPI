using AutoMapper;
using ResignationAPI.Models;
using ResignationAPI.Models.DTOs;

namespace ResignationAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Resignation, ResignationRequestDTO>().ReverseMap();
            CreateMap<Resignation, ResignationDTO>().ReverseMap();
        }
    }
}
