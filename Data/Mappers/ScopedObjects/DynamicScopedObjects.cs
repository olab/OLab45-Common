using OLab.Api.Common;
using OLab.Api.Dto;
using OLab.Common.Interfaces;
using OLab.Api.WikiTag;

namespace OLab.Api.ObjectMapper;

public class DynamicScopedObjects : ObjectMapper<OLab.Data.BusinessObjects.DynamicScopedObjects, DynamicScopedObjectsDto>
{
  protected readonly bool enableWikiTranslation = true;

  public DynamicScopedObjects(IOLabLogger logger, bool enableWikiTranslation = true) : base(logger)
  {
  }

  public DynamicScopedObjects(IOLabLogger logger, WikiTagModuleProvider tagProvider, bool enableWikiTranslation = true) : base(logger, tagProvider)
  {
  }

  public override DynamicScopedObjectsDto PhysicalToDto(OLab.Data.BusinessObjects.DynamicScopedObjects phys, object source = null)
  {
    throw new System.NotImplementedException();
  }

  public override OLab.Data.BusinessObjects.DynamicScopedObjects DtoToPhysical(DynamicScopedObjectsDto dto, object source = null)
  {
    throw new System.NotImplementedException();
  }

  public DynamicScopedObjectsDto PhysicalToDto(OLab.Data.BusinessObjects.DynamicScopedObjects phys)
  {
    var dto = new DynamicScopedObjectsDto();
    PhysicalToDto(phys, dto);
    return dto;
  }

  public void PhysicalToDto(
    OLab.Data.BusinessObjects.DynamicScopedObjects phys,
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