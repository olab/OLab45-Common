using AutoMapper;
using OLab.Api.Common;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Mappers;
using OLab.Data.Models;
using System.Text;

namespace OLab.Data.Mappers
{
  public class ConstantsFullMapper : OLabMapper<SystemConstants, ConstantsDto>
  {

    public ConstantsFullMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public ConstantsFullMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
        cfg.CreateMap<string, byte[]>().ConvertUsing(s => Encoding.ASCII.GetBytes(s));
        cfg.CreateMap<byte[], string>().ConvertUsing(s => Encoding.ASCII.GetString(s));
        cfg.CreateMap<SystemConstants, ConstantsDto>().ReverseMap();
      });
    }

  }
}