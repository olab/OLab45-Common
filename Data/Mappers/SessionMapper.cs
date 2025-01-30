using AutoMapper;
using OLab.Api.Model;
using OLab.Api.ObjectMapper;
using OLab.Api.WikiTag;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Session;
using System;

namespace OLab.Data.Mappers;

public class SessionMapper : OLabMapper<UserSessions, SessionDto>
{
  public SessionMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base( logger, dbContext, tagProvider )
  {
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration( cfg =>
    {
      cfg.CreateMap<UserSessions, SessionDto>().ReverseMap();
      cfg.CreateMap<decimal, DateTime>().ConvertUsing( new DateTimeTypeConverter() );
      cfg.CreateMap<DateTime, decimal>().ConvertUsing( new DecimalDateTimeTypeConverter() );
    } );
  }

}
