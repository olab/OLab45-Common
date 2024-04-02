using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLab.Api.ObjectMapper;

public class Constants : OLabMapper<SystemConstants, ConstantsDto>
{
  public Constants(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public Constants(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override ConstantsDto PhysicalToDto(SystemConstants phys, ConstantsDto dto)
  {
    dto.Value = Encoding.UTF8.GetString(phys.Value);
    return dto;
  }

  public override SystemConstants DtoToPhysical(ConstantsDto dto, SystemConstants phys)
  {
    phys.Value = Encoding.UTF8.GetBytes(dto.Value);
    return phys;
  }

  /// <summary>
  /// Create a physical object from an element array
  /// </summary>
  /// <param name="elements">List of elements</param>
  /// <param name="source">Physical object</param>
  /// <returns>Physical object</returns>
  public override SystemConstants ElementsToPhys(IEnumerable<dynamic> elements, Object source = null)
  {
    var phys = GetPhys(source);

    phys.Id = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
    CreateIdTranslation(phys.Id);

    if (uint.TryParse(elements.FirstOrDefault(x => x.Name == "map_id").Value, out uint id))
      phys.ImageableId = id;
    phys.ImageableType = Utils.Constants.ScopeLevelMap;

    dynamic value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "mime"));
    phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"));

    value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "path"));

    phys.CreatedAt = DateTime.Now;

    return phys;
  }
}
