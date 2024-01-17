using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Data.Mappers;

public class MapVpdElementMapper : OLabMapper<MapVpdElements, MapVpdElementsDto>
{
  public MapVpdElementMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public override MapVpdElements ElementsToPhys(IEnumerable<dynamic> elements, object source = null)
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
