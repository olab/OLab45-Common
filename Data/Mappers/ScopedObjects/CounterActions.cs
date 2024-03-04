using AutoMapper;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.ObjectMapper;

public class CounterActionsMapper : OLabMapper<SystemCounterActions, CounterActionsDto>
{
  public static int DefaultWidth = 400;
  public static int DefaultHeight = 300;

  public CounterActionsMapper(
    IOLabLogger logger,
    bool enableWikiTranslation = true) : base(logger)
  {
  }

  /// <summary>
  /// Default (overridable) AutoMapper cfg
  /// </summary>
  /// <returns>MapperConfiguration</returns>
  protected override MapperConfiguration GetConfiguration()
  {
    return new MapperConfiguration(cfg =>
     cfg.CreateMap<SystemCounterActions, CounterActionsDto>()
      .ForMember(dest => dest.NodeId, act => act.MapFrom(src => src.ImageableId))
      .ForMember(dest => dest.Function, act => act.MapFrom(src => src.Expression))
      .ForMember(dest => dest.Display, act => act.MapFrom(src => src.Visible))
      .ReverseMap()
    );
  }

  public override SystemCounterActions ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
  {
    var phys = GetPhys(source);

    phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    phys.OperationType = "open";

    phys.ImageableId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "node_id").Value);
    phys.ImageableType = Utils.Constants.ScopeLevelNode;
    phys.CounterId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "counter_id").Value);
    phys.Expression = elements.FirstOrDefault(x => x.Name == "function").Value;
    phys.Visible = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "display").Value);

    return phys;
  }

}
