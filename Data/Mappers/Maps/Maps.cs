using AutoMapper;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.ObjectMapper;

public class MapsMapper : OLabMapper<Maps, Dto.MapsDto>
{
  public MapsMapper(
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
     cfg.CreateMap<Maps, Dto.MapsDto>()
      .ForMember(dest => dest.Description, act => act.MapFrom(src => src.Abstract))
      .ReverseMap()
    );
  }

  public override Maps ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
  {
    var phys = base.GetPhys(source);

    phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"));
    phys.AuthorId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "author_id").Value);
    phys.Abstract = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "abstract"));
    phys.StartScore = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "startScore").Value);
    phys.Threshold = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "threshold").Value);
    phys.Keywords = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "keywords"));
    phys.TypeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "type_id").Value);
    phys.Units = elements.FirstOrDefault(x => x.Name == "units").Value;
    phys.SecurityId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "security_id").Value);
    phys.Guid = elements.FirstOrDefault(x => x.Name == "guid").Value;
    phys.Timing = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "timing").Value) == 1;
    phys.DeltaTime = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "delta_time").Value);
    phys.ReminderMsg = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "reminder_msg"));
    phys.ReminderTime = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "reminder_time").Value);
    phys.ShowBar = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "show_bar").Value) == 1;
    phys.ShowScore = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "show_score").Value) == 1;
    phys.SkinId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "skin_id").Value);
    phys.Enabled = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "enabled").Value) == 1;
    phys.SectionId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "section_id").Value);
    phys.LanguageId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "language_id").Value);
    phys.Feedback = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "feedback"));
    phys.DevNotes = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "dev_notes"));
    phys.Source = elements.FirstOrDefault(x => x.Name == "source").Value;
    phys.SourceId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "source_id").Value);
    phys.Verification = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "verification"));
    phys.AuthorRights = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "author_rights").Value);
    phys.RevisableAnswers = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "revisable_answers").Value) == 1;
    phys.SendXapiStatements = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "send_xapi_statements").Value) == 1;
    phys.CreatedAt = TimeUtils.UtcNow();

    if (elements.Any(x => x.Name == "report_node_id"))
      phys.ReportNodeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "report_node_id").Value);

    return phys;
  }
}
