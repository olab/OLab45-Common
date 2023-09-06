using AutoMapper;
using OLab.Common;
using OLab.Dto.Designer;
using OLab.Model;
using OLab.Utils;

namespace OLab.ObjectMapper.Designer
{
  public class Files : OLabMapper<SystemFiles, ScopedObjectDto>
  {
    public Files(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public Files(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
    public override ScopedObjectDto PhysicalToDto(SystemFiles phys, ScopedObjectDto dto)
    {
      if (string.IsNullOrEmpty(phys.Name))
        dto.Wiki = $"[[MR:{phys.Id}]]";
      else
        dto.Wiki = $"[[MR:{phys.Name}]]";
      return dto;
    }

    /// <summary>
    /// Default (overridable) AutoMapper configuration
    /// </summary>
    /// <returns>MapperConfiguration</returns>
    protected override MapperConfiguration GetConfiguration()
    {
      return new MapperConfiguration(cfg =>
       cfg.CreateMap<SystemFiles, ScopedObjectDto>()
        .ForMember(dest => dest.ScopeLevel, act => act.MapFrom(src => src.ImageableType))
        .ForMember(dest => dest.ParentId, act => act.MapFrom(src => src.ImageableId))
        .ReverseMap()
      );
    }
  }
}