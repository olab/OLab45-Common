using AutoMapper;
using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace OLab.Data.Mappers
{
  public class CountersFullMapper : OLabMapper<SystemCounters, CountersFullDto>
  {

    public CountersFullMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public CountersFullMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
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
    public override CountersFullDto PhysicalToDto(SystemCounters phys, CountersFullDto source)
    {
      if (phys.Value == null)
        phys.Value = new List<byte>().ToArray();
      if (phys.StartValue == null)
        phys.StartValue = new List<byte>().ToArray();
      return source;
    }

    /// <summary>
    /// Convert a dto object to a specific physicla one. 
    /// </summary>
    /// <remarks>
    /// Allows for derived class specific overrides that 
    /// don't fit well with default implementation
    /// </remarks>
    /// <param name="dto">Ddto object</param>
    /// <param name="source">Base physical object</param>
    /// <returns>Physical object</returns>
    public override SystemCounters DtoToPhysical(CountersFullDto dto, SystemCounters source)
    {
      if (string.IsNullOrEmpty(dto.StartValue))
        source.StartValue = BitConverter.GetBytes(0);
      if (string.IsNullOrEmpty(dto.Value))
        source.Value = BitConverter.GetBytes(0);
      return source;
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
        cfg.CreateMap<SystemCounters, CountersFullDto>().ReverseMap();
      });
    }

  }
}