using AutoMapper;
using Newtonsoft.Json;
using Survey.API.Models;
using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;

namespace Survey.API.Helpers;

public class DTOsMapper : Profile
{
    public DTOsMapper()
    {
        CreateMap<SurveyRequest, SurveyDTO>()
        .ForMember(dest => dest.code, opt => opt.MapFrom(src => "000"))
        .ForMember(dest => dest.message, opt => opt.MapFrom(src => "success"))
        .ForMember(dest => dest.data, opt => opt.MapFrom(src => new List<SurveyRequest> { src }));

        CreateMap<SurveyDTO, SurveyModel>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
        .ForMember(dest => dest.respCode, opt => opt.MapFrom(src => src.code))
        .ForMember(dest => dest.jsonData, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.data)))
        .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => DateTime.Now));
    }
}
