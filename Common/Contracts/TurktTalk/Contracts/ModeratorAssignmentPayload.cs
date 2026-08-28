using OLab.Common.Contracts.TurktTalk.BusinessObjects;
using System.Collections.Generic;

namespace OLab.Common.Contracts.TurktTalk.Contracts;

public class ModeratorAssignmentPayload
{
  public IList<MapNodeListItem> MapNodes { get; set; }
  public Moderator Remote { get; set; }

}
