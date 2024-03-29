using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Text;

namespace OLab.Api.ObjectMapper;

public class ConstantsFull : OLabMapper<SystemConstants, ConstantsDto>
{
  public ConstantsFull(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {

    public ConstantsFull(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public ConstantsFull(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
      cfg.CreateMap<string, byte[]>().ConvertUsing(s => Encoding.UTF8.GetBytes(s));
      cfg.CreateMap<byte[], string>().ConvertUsing(s => Encoding.UTF8.GetString(s));
      cfg.CreateMap<SystemConstants, ConstantsDto>().ReverseMap();
    });
  }
}
