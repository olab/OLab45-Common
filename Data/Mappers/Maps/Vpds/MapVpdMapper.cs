using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Data.Mappers;

public class MapVpdMapper : OLabMapper<MapVpds, MapVpdsDto>
{
  public MapVpdMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public override MapVpds ElementsToPhys(IEnumerable<dynamic> elements, object source = null)
  {
    var phys = GetPhys(source);

    phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    CreateIdTranslation(phys.Id);
    phys.MapId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "map_id").Value);
    phys.VpdTypeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "vpd_type_id").Value);

    // Logger.LogInformation($"loaded MapVpd {phys.Id}");

    return phys;
  }
}
