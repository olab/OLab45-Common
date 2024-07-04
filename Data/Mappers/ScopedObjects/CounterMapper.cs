using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class CounterValueResolver : IValueResolver<SystemCounters, CountersDto, string>
{
  public string Resolve(SystemCounters source, CountersDto destination, string destMember, ResolutionContext context)
  {
    return source.ValueAsString();
  }
}

public class CounterMapper : OLabMapper<SystemCounters, CountersDto>
{

  public CounterMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider = null,
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
      cfg.CreateMap<SystemCounters, CountersDto>()
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => src.ValueAsString()));

      cfg.CreateMap<CountersDto, SystemCounters>()
                .ForMember(dst => dst.Value, opt => opt.MapFrom(src => Encoding.ASCII.GetBytes(src.Value)));
    });
  }

  public override CountersDto PhysicalToDto(SystemCounters phys, CountersDto dto)
  {
    if (phys.Value != null)
      dto.Value = Encoding.ASCII.GetString(phys.Value);
    dto.Value ??= "";
    dto.Description = Conversions.Base64Decode(phys.Description);

    return dto;
  }

  public override SystemCounters DtoToPhysical(CountersDto dto, SystemCounters phys)
  {
    phys.Value = Encoding.ASCII.GetBytes(dto.Value);
    return phys;
  }

  public override SystemCounters ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
  {
    var phys = GetPhys(source);

    phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"));
    phys.Description = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "description"));
    phys.StartValue = Encoding.ASCII.GetBytes(elements.FirstOrDefault(x => x.Name == "start_value").Value);
    phys.Value = phys.StartValue;
    phys.Visible = Convert.ToInt16(elements.FirstOrDefault(x => x.Name == "visible").Value) == 1;
    phys.CreatedAt = DateTime.Now;

    return phys;
  }
}