using OLab.Data.Dtos;
using System.Collections.Generic;

namespace OLab.Data.Contracts
{
  public class ExtendMapResponse
  {
    public IList<MapNodesFullDto> Nodes { get; set; }
    public IList<MapNodeLinksDto> Links { get; set; }

    public ExtendMapResponse()
    {
      Nodes = new List<MapNodesFullDto>();
      Links = new List<MapNodeLinksDto>();
    }
  }
}