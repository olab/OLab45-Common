using OLab.TurkTalk.BusinessObjects;
using System.Collections.Generic;

namespace OLab.Common.Contracts
{
  public class ModeratorAssignmentPayload
  {
    public IList<MapNodeListItem> MapNodes { get; set; }
    public Moderator Remote { get; set; }

  }
}
