using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Api.Utils;

namespace OLab.Api.ObjectMapper
{
  public class DynamicScopedObjects : ObjectMapper<Model.ScopedObjects, DynamicScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    public DynamicScopedObjects(OLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public DynamicScopedObjects(OLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override DynamicScopedObjectsDto PhysicalToDto(Model.ScopedObjects phys, object source = null)
    {
      throw new System.NotImplementedException();
    }

    public override Model.ScopedObjects DtoToPhysical(DynamicScopedObjectsDto dto, object source = null)
    {
      throw new System.NotImplementedException();
    }

    public DynamicScopedObjectsDto PhysicalToDto(
      Model.ScopedObjects server,
      Model.ScopedObjects map,
      Model.ScopedObjects node
    )
    {
      var dto = new DynamicScopedObjectsDto();
      PhysicalToDto(server, map, node, dto);
      return dto;
    }

    public void PhysicalToDto(
      Model.ScopedObjects server,
      Model.ScopedObjects map,
      Model.ScopedObjects node,
      DynamicScopedObjectsDto dto)
    {
      System.Collections.Generic.IList<CountersDto> dtoCountersList = new ObjectMapper.Counters(logger, GetWikiProvider()).PhysicalToDto(server.Counters);
      dto.Server.Counters.AddRange(dtoCountersList);

      dtoCountersList = new ObjectMapper.Counters(logger, GetWikiProvider()).PhysicalToDto(map.Counters);
      dto.Map.Counters.AddRange(dtoCountersList);

      dtoCountersList = new ObjectMapper.Counters(logger, GetWikiProvider()).PhysicalToDto(node.Counters);
      dto.Node.Counters.AddRange(dtoCountersList);

      // var dtoConstantsList = new ConstantsObjectMapper(logger, GetWikiProvider()).PhysicalToDto( server.Constants );
      // dto.Server.Constants.AddRange(dtoConstantsList);

      // calculate validation
      dto.Checksum = dto.GenerateChecksum();
    }

  }
}