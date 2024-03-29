using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto.Designer;
using OLab.Api.Model;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper.Designer;

public class Questions : OLabMapper<SystemQuestions, ScopedObjectDto>
{
  public Questions(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public Questions(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
  public override ScopedObjectDto PhysicalToDto(SystemQuestions phys, ScopedObjectDto dto)
  {
    if (string.IsNullOrEmpty(phys.Name))
      dto.Wiki = $"[[QU:{phys.Id}]]";
    else
      dto.Wiki = $"[[QU:{phys.Name}]]";
    return dto;
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
     cfg.CreateMap<SystemQuestions, ScopedObjectDto>()
      .ForMember(dest => dest.ScopeLevel, act => act.MapFrom(src => src.ImageableType))
      .ForMember(dest => dest.ParentId, act => act.MapFrom(src => src.ImageableId))
      .ReverseMap()
    );
  }
}