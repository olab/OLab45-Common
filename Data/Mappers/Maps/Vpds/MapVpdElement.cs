using OLabWebAPI.Common;
using OLabWebAPI.Dto;
using OLabWebAPI.Model;
using OLabWebAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLabWebAPI.ObjectMapper
{
  public class MapVpdElement : OLabMapper<MapVpdElements, MapVpdElementsDto>
  {
    public MapVpdElement(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override MapVpdElements ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
    {
      MapVpdElements phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      CreateIdTranslation(phys.Id);
      phys.VpdId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "vpd_id").Value);
      phys.Key = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "key"));
      phys.Value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "value"));

      // logger.LogInformation($"loaded MapVpdElement {phys.Id}");

      return phys;
    }
  }
}
