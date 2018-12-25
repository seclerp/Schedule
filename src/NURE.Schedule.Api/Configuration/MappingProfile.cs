using AutoMapper;
using NURE.Schedule.Common;
using NURE.Schedule.Domain.CistApi.Structure;
using NURE.Schedule.Domain.Entities;
using NURE.Schedule.Services.Models;

namespace NURE.Schedule.Api.Configuration
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<SearchItemEntity, SearchResultModel>()
        .ForMember(x => x.Value, y => y.MapFrom(s => s.FullValue))
        .ForMember(x => x.ShortValue, y => y.MapFrom(s => s.Value))
        .ForMember(x => x.TimeTableId, y => y.MapFrom(s => s.Id));

      CreateMap<Teacher, SearchItemEntity>()
        .ForMember(x => x.Value, y => y.MapFrom(s => s.ShortName))
        .ForMember(x => x.FullValue, y => y.MapFrom(s => s.FullName))
        .ForMember(x => x.ItemType, y => y.MapFrom(_ => SearchItemType.Teacher));

      CreateMap<Group, SearchItemEntity>()
        .ForMember(x => x.Value, y => y.MapFrom(s => s.Name))
        .ForMember(x => x.FullValue, y => y.MapFrom(s => s.Name))
        .ForMember(x => x.ItemType, y => y.MapFrom(_ => SearchItemType.Group));
    }
  }
}