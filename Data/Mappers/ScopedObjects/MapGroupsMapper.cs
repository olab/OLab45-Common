using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class MapGrouprolesMapper : OLabMapper<MapGrouproles, MapGrouprolesDto>
{
  private readonly OLabDBContext dbContext;

  public MapGrouprolesMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    bool enableWikiTranslation = true) : base(logger, dbContext)
  {
    this.dbContext = dbContext;
  }

  public MapGrouprolesMapper(
    IOLabLogger logger,
    OLabDBContext dbContext,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, dbContext, tagProvider)
  {
  }

  public override MapGrouprolesDto PhysicalToDto(MapGrouproles phys)
  {
    var dto = base.PhysicalToDto(phys);

    if (phys.Group != null)
      dto.GroupName = phys.Group.Name;

    if (phys.Role != null)
      dto.RoleName = phys.Role.Name;

    return dto;
  }

  public MapGrouproles DtoToPhysical(uint mapId, MapGrouprolesDto dto)
  {
    var mapGroupPhys = new MapGrouproles
    {
      Id = dto.Id,
      MapId = mapId,
      GroupId = dto.Id,
      RoleId = dto.RoleId
    };

    return mapGroupPhys;
  }

  public IList<MapGrouproles> DtoToPhysical(uint mapId, IList<MapGrouprolesDto> dtos)
  {
    var MapGrouprolesPhys = new List<MapGrouproles>();
    foreach (var dto in dtos)
      MapGrouprolesPhys.Add(DtoToPhysical(mapId, dto));

    return MapGrouprolesPhys;
  }

  public override MapGrouproles DtoToPhysical(MapGrouprolesDto dto)
  {
    var phys = base.DtoToPhysical(dto);

    if (dto.GroupId.HasValue)
    {
      phys.Group = dbContext.Groups.FirstOrDefault(x => x.Id == dto.GroupId.Value);
      if (phys.Group == null)
        throw new OLabObjectNotFoundException("Groups", dto.GroupId.Value);
    }

    if (dto.RoleId.HasValue)
    {
      phys.Role = dbContext.Roles.FirstOrDefault(x => x.Id == dto.RoleId.Value);
      if (phys.Role == null)
        throw new OLabObjectNotFoundException("Roles", dto.RoleId.Value);
    }

    return phys;
  }
}