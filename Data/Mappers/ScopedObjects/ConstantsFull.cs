using AutoMapper;
using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;
using System.Text;

namespace OLab.ObjectMapper
{
  public class ConstantsFull : OLabMapper<SystemConstants, ConstantsDto>
  {

    public ConstantsFull(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public ConstantsFull(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    /// <summary>
    /// Default (overridable) AutoMapper configuration
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