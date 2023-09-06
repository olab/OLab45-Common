using OLab.Common;
using OLab.Dto;
using OLab.Model;
using OLab.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OLab.ObjectMapper
{
  public class MapVpd : OLabMapper<MapVpds, MapVpdsDto>
  {
    public MapVpd(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override MapVpds ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
    {
      MapVpds phys = GetPhys(source);

      phys.Id = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      CreateIdTranslation(phys.Id);
      phys.MapId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "map_id").Value);
      phys.VpdTypeId = Convert.ToUInt32(elements.FirstOrDefault(x => x.Name == "vpd_type_id").Value);

      // logger.LogInformation($"loaded MapVpd {phys.Id}");

      return phys;
    }
  }
}
