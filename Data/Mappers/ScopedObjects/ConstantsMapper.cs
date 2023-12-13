using OLab.Api.Common;
using OLab.Api.Utils;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OLab.Data.Mappers
{
  public class ConstantsMapper : OLabMapper<SystemConstants, ConstantsDto>
  {
    public ConstantsMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public ConstantsMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override ConstantsDto PhysicalToDto(SystemConstants phys, ConstantsDto dto)
    {
      dto.Value = Encoding.ASCII.GetString(phys.Value);
      return dto;
    }

    public override SystemConstants DtoToPhysical(ConstantsDto dto, SystemConstants phys)
    {
      phys.Value = Encoding.ASCII.GetBytes(dto.Value);
      return phys;
    }

    /// <summary>
    /// Create a physical object from an element array
    /// </summary>
    /// <param name="elements">List of elements</param>
    /// <param name="source">Physical object</param>
    /// <returns>Physical object</returns>
    public override SystemConstants ElementsToPhys(IEnumerable<dynamic> elements, object source = null)
    {
      var phys = GetPhys(source);

      phys.Id = Convert.ToInt32(elements.FirstOrDefault(x => x.Name == "id").Value);
      CreateIdTranslation(phys.Id);

      if (uint.TryParse(elements.FirstOrDefault(x => x.Name == "map_id").Value, out uint id))
        phys.ImageableId = id;
      phys.ImageableType = ConstantStrings.ScopeLevelMap;

      dynamic value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "mime"));
      phys.Name = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "name"));

      value = Conversions.Base64Decode(elements.FirstOrDefault(x => x.Name == "path"));

      phys.CreatedAt = DateTime.Now;

      return phys;
    }
  }
}