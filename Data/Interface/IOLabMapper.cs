using OLab.Api.WikiTag;
using System.Collections.Generic;

namespace OLab.Data.Interface;

public interface IOLabMapper<P, D>
  where P : new()
  where D : new()
{
  P DtoToPhysical(D dto);
  P DtoToPhysical(D dto, P source);
  IList<P> DtoToPhysical(IList<D> dtoList);
  P ElementsToPhys(IEnumerable<dynamic> elements, object source = null);
  D GetDto(object source = null);
  P GetPhys(object source = null);
  WikiTagModuleProvider GetWikiProvider();
  IList<D> PhysicalToDto(IList<P> physList);
  D PhysicalToDto(P phys);
  D PhysicalToDto(P phys, D source);
}