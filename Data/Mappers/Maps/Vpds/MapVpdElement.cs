using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using OLab.Common.Interfaces;

namespace OLab.Api.ObjectMapper
{
    public class MapVpdElement : OLabMapper<MapVpdElements, MapVpdElementsDto>
  {
    public MapVpdElement(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override MapVpdElements ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
    {
      var phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      CreateIdTranslation(phys.Id);
      phys.VpdId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "vpd_id").Value);
      phys.Key = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "key"));
      phys.Value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "value"));

      // Logger.LogInformation($"loaded MapVpdElement {phys.Id}");

      return phys;
    }
  }
}
