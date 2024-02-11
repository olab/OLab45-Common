using OLab.Api.TurkTalk.BusinessObjects;
using System.Collections.Generic;

namespace OLab.Api.Common.Contracts;

public class ModeratorAssignmentPayload
{
  public IList<MapNodeListItem> MapNodes { get; set; }
  public Moderator Remote { get; set; }

}
