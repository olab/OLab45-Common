using AutoMapper;
using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos.Session;
using OLab.Data.Models;
using System;

namespace OLab.Data.Mappers;

public class SessionMapper : OLabMapper<UserSessions, SessionDto>
{
  public SessionMapper(
    IOLabLogger logger,
    bool enableWikiTranslation = true) : base(logger)
  {
  }
  public SessionMapper(
    IOLabLogger logger,
    WikiTagProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, tagProvider)
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
      cfg.CreateMap<UserSessions, SessionDto>().ReverseMap();
      cfg.CreateMap<decimal, DateTime>().ConvertUsing(new DateTimeTypeConverter());
      cfg.CreateMap<DateTime, decimal>().ConvertUsing(new DecimalDateTimeTypeConverter());
    });
  }

}
