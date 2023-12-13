using OLab.Api.Common;
using OLab.Api.Models;
using OLab.Common.Interfaces;
using OLab.Data.Dtos;
using OLab.Data.Mappers;

namespace OLab.Data.Mappers
{
  public class DynamicScopedObjectsMapper : ObjectMapper<DynamicScopedObjects, DynamicScopedObjectsDto>
  {
    protected readonly bool enableWikiTranslation = true;

    public DynamicScopedObjectsMapper(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
    {
    }

    public DynamicScopedObjectsMapper(IOLabLogger logger, WikiTagProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
    {
    }

    public override DynamicScopedObjectsDto PhysicalToDto(DynamicScopedObjects phys, object source = null)
    {
      throw new System.NotImplementedException();
    }

    public override DynamicScopedObjects DtoToPhysical(DynamicScopedObjectsDto dto, object source = null)
    {
      throw new System.NotImplementedException();
    }

    public DynamicScopedObjectsDto PhysicalToDto(DynamicScopedObjects phys)
    {
      var dto = new DynamicScopedObjectsDto();
      PhysicalToDto(phys, dto);
      return dto;
    }

    public void PhysicalToDto(
      DynamicScopedObjects phys,
      DynamicScopedObjectsDto dto)
    {
      var dtoCountersList = new CounterMapper(Logger).PhysicalToDto(phys.ServerCounters);
      dto.Server.Counters.AddRange(dtoCountersList);

      dtoCountersList = new CounterMapper(Logger).PhysicalToDto(phys.MapCounters);
      dto.Map.Counters.AddRange(dtoCountersList);

      dtoCountersList = new CounterMapper(Logger).PhysicalToDto(phys.NodeCounters);
      dto.Node.Counters.AddRange(dtoCountersList);

      // var dtoConstantsList = new ConstantsObjectMapper(Logger, GetWikiProvider()).PhysicalToDto( server.ConstantsPhys );
      // dto.Server.ConstantsPhys.AddRange(dtoConstantsList);

      // calculate validation
      dto.Checksum = dto.GenerateChecksum();
    }

  }
}