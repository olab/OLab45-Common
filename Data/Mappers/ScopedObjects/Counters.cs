using AutoMapper;
using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLabWebAPI.ObjectMapper
{
  public class CounterValueResolver : IValueResolver<SystemCounters, CountersDto, string>
  {
    public string Resolve(SystemCounters source, CountersDto destination, string destMember, ResolutionContext context)
    {
      return source.ValueAsString();
    }
  }

  public class Counters : OLabMapper<SystemCounters, CountersDto>
  {
    public Counters(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public Counters(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
      if (dto.Value == null)
        dto.Value = "";
      return dto;
    }

    public override SystemCounters DtoToPhysical(CountersDto dto, SystemCounters phys)
    {
      phys.Value = Encoding.ASCII.GetBytes(dto.Value);
      return phys;
    }

    public override SystemCounters ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
    {
      SystemCounters phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"));
      phys.Description = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "description"));
      phys.StartValue = Encoding.ASCII.GetBytes(elements.FirstOrDefault(x => x.Name == "start_value").Value);
      phys.CreatedAt = DateTime.Now;

      return phys;
    }
  }
}