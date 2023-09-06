using OLab.Api.Dto;
using System.Collections.Generic;

namespace OLab.Api.Model
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