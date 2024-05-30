using OLab.Api.Common;
using OLab.Api.Data.Exceptions;
using OLab.Api.Dto;
using OLab.Api.Model;
using OLab.Common.Interfaces;
using System.IO;
using System.Linq;

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
    WikiTagProvider tagProvider,
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

  public override MapGroups DtoToPhysical(MapGroupsDto dto)
  {
    var phys = base.DtoToPhysical(dto);

    phys.Group = dbContext.Groups.FirstOrDefault(x => x.Id == dto.GroupId);
    if (phys.Group == null)
      throw new OLabObjectNotFoundException("Groups", dto.GroupId);

    phys.Map = dbContext.Maps.FirstOrDefault(x => x.Id == dto.MapId);
    if (phys.Map == null)
      throw new OLabObjectNotFoundException("Maps", dto.MapId);

    return phys;
  }
}