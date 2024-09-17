using AutoMapper;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using System.Text;

namespace OLab.Api.ObjectMapper;

public class ConstantsFull : OLabMapper<SystemConstants, ConstantsDto>
{

  public ConstantsFull(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  public ConstantsFull(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
    {
      cfg.CreateMap<string, byte[]>().ConvertUsing(s => Encoding.UTF8.GetBytes(s));
      cfg.CreateMap<byte[], string>().ConvertUsing(s => Encoding.UTF8.GetString(s));
      cfg.CreateMap<SystemConstants, ConstantsDto>().ReverseMap();
    });
  }

}