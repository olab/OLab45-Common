using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.Linq;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class MapGroupsMapper : OLabMapper<MapGroups, MapGroupsDto>
{
  private readonly OLabDBContext dbContext;

  public MapGroupsMapper(
  IOLabLogger logger,
  OLabDBContext dbContext,
  bool enableWikiTranslation = true) : base(logger)
  {
    this.dbContext = dbContext;
  }

  public MapGroupsMapper(
    IOLabLogger logger,
    WikiTagModuleProvider tagProvider,
    bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override MapGroupsDto PhysicalToDto(MapGroups phys)
  {
    var dto = base.PhysicalToDto(phys);

    if (phys.Group != null)
      dto.GroupName = phys.Group.Name;

    return dto;
  }

  public MapGroups DtoToPhysical(uint mapId, GroupsDto dto)
  {
    var mapGroupPhys = new MapGroups
    {
      MapId = mapId,
      GroupId = dto.Id
    };

    return mapGroupPhys;
  }

  public IList<MapGroups> DtoToPhysical(uint mapId, IList<GroupsDto> dtos)
  {
    var mapGroupsPhys = new List<MapGroups>();
    foreach (var dto in dtos)
      mapGroupsPhys.Add(DtoToPhysical(mapId, dto));

    return mapGroupsPhys;
  }

  public override MapGroups DtoToPhysical(MapGroupsDto dto)
  {
    var phys = base.DtoToPhysical(dto);

    phys.Group = dbContext.Groups.FirstOrDefault(x => x.Id == dto.GroupId);
    if (phys.Group == null)
      throw new OLabObjectNotFoundException("Groups", dto.GroupId);

    return phys;
  }
}