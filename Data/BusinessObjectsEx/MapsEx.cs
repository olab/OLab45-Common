using AutoMapper;
using System.Collections.Generic;

#nullable disable

namespace OLab.Model
{
  public partial class Maps
  {

    public static Maps CreateDefault(Maps templateMap)
    {
      // use AutoMapper to do a Deep Copy
      var mapper = new Mapper(new MapperConfiguration(cfg =>
        cfg.CreateMap<Maps, Maps>().ReverseMap()
      ));

      Maps map = mapper.Map<Maps>(templateMap);

      map.Id = 0;
      map.IsTemplate = 0;

      return map;
    }

    public void AppendMapNodes(Maps sourceMap)
    {
      foreach (MapNodes node in sourceMap.MapNodes)
        MapNodes.Add(node);
    }

    public static Maps CreateDefault()
    {
      var map = new Maps
      {
        Abstract = "<b>New Map</b>",
        DeltaTime = 0,
        DevNotes = "",
        Enabled = true,
        Feedback = "",
        Guid = "",
        IsTemplate = 0,
        Keywords = "",
        LanguageId = 1,
        Name = "New Map",
        ReminderMsg = "",
        ReminderTime = 0,
        RendererVersion = 4,
        SectionId = 2,
        SecurityId = 3,
        SendXapiStatements = false,
        ShowBar = false,
        ShowScore = false,
        SkinId = 1,
        SourceId = 0,
        Source = "",
        TypeId = 11,
        Units = "",
        Verification = "{}",
        MapNodes = new List<MapNodes>(),
        ReportNodeId = 0
      };

      map.MapNodes.Add(new MapNodes { Title = "New Node", TypeId = 1, Text = "Sample Text" });

      return map;
    }
  }
}
