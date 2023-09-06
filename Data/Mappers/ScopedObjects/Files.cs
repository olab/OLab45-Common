using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Api.ObjectMapper
{
  public class Files : OLabMapper<SystemFiles, FilesDto>
  {
    public Files(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public Files(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override SystemFiles ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
    {
      SystemFiles phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      if (uint.TryParse(elements.FirstOrDefault(x => x.Name == "map_id").Value, out uint id))
        phys.ImageableId = id;
      phys.Mime = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "mime"));

      phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"), true);
      phys.Path = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "path"));
      phys.Args = elements.FirstOrDefault(x => x.Name == "args").Value;

      phys.Width = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "width").Value);
      phys.WidthType = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "width_type"));
      phys.Height = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "height").Value);
      phys.HeightType = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "height_type"));

      phys.HAlign = elements.FirstOrDefault(x => x.Name == "h_align").Value;
      phys.VAlign = elements.FirstOrDefault(x => x.Name == "v_align").Value;

      phys.IsShared = Convert.ToSByte(elements.FirstOrDefault(x => x.Name == "is_shared").Value);
      phys.IsPrivate = Convert.ToSByte(elements.FirstOrDefault(x => x.Name == "is_private").Value);
      phys.CreatedAt = DateTime.Now;

      // logger.LogInformation($"loaded SystemFiles {phys.Id}");

      return phys;
    }

  }
}