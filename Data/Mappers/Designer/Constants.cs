using AutoMapper;
using OLab.Api.Dto.Designer;
using OLab.Common.Interfaces;
using OLab.Data.BusinessObjects;

namespace OLab.Api.ObjectMapper.Designer
{
    public class Constants : OLabMapper<SystemConstants, ScopedObjectDto>
  {
    public Constants(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    /// <summary>
    /// Convert a physical object to a specific dto. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="phys">Physical object</param>
    /// <param name="source">Base dto object</param>
    /// <returns>Dto object</returns>
    public override ScopedObjectDto PhysicalToDto(SystemConstants phys, ScopedObjectDto dto)
    {
      if (string.IsNullOrEmpty(phys.Name))
        dto.Wiki = $"[[CONST:{phys.Id}]]";
      else
        dto.Wiki = $"[[CONST:{phys.Name}]]";
      return dto;
    }

    /// <summary>
    /// Default (overridable) AutoMapper cfg
    /// </summary>
    /// <returns>MapperConfiguration</returns>
    protected override MapperConfiguration GetConfiguration()
    {
      return new MapperConfiguration(cfg =>
       cfg.CreateMap<SystemConstants, ScopedObjectDto>()
        .ForMember(dest => dest.ScopeLevel, act => act.MapFrom(src => src.ImageableType))
        .ForMember(dest => dest.ParentId, act => act.MapFrom(src => src.ImageableId))
        .ReverseMap()
      );
    }
  }
}