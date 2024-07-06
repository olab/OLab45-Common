using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using OLab.Api.WikiTag;
using OLab.Api.Model;

namespace OLab.Api.ObjectMapper;

public class MapNodesMapper : OLabMapper<Model.MapNodes, MapNodesDto>
{
  public static int DefaultWidth = 400;
  public static int DefaultHeight = 300;

  public MapNodesMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    IOLabModuleProvider<IWikiTagModule> tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  public override Model.MapNodes ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
  {
    var phys = GetPhys(source);

    phys.Annotation = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "annotation"));
    phys.Conditional = elements.FirstOrDefault(x => x.Name == "conditional").Value;
    phys.ConditionalMessage = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "conditional_message"));

    phys.End = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "end").Value) == 1 ? true : false;
    phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    phys.Info = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "info"));
    phys.IsPrivate = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "is_private").Value);
    phys.Kfp = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "kfp").Value) == 1 ? true : false;
    phys.LinkStyleId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "link_style_id").Value);
    phys.LinkTypeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "link_type_id").Value);
    phys.MapId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "map_id").Value);
    phys.PriorityId = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "priority_id").Value);
    phys.Probability = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "probability").Value) == 1 ? true : false;
    phys.Rgb = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "rgb"));
    phys.ShowInfo = Convert.ToSByte(elements.FirstOrDefault(x => x.Name == "show_info").Value);
    phys.Text = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "text"));
    phys.Title = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "title"));
    phys.TypeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "type_id").Value);
    phys.Undo = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "undo").Value) == 1 ? true : false;
    phys.X = Convert.ToDouble(elements.FirstOrDefault(x => x.Name == "x").Value);
    phys.Y = Convert.ToDouble(elements.FirstOrDefault(x => x.Name == "y").Value);
    phys.CreatedAt = DateTime.Now;

    // Logger.LogInformation($"loaded MapNodes {phys.Id}");

    return phys;
  }

}
