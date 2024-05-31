using OLab.Api.TurkTalk.BusinessObjects;
using OLab.Api.TurkTalk.Contracts;
using OLab.Api.TurkTalk.Methods;
using System.Collections.Generic;

namespace OLab.Api.TurkTalk.Commands;

public class ModeratorAssignmentCommand : CommandMethod
{
  public ModeratorAssignmentPayload Data { get; set; }

  public Moderator Remote { get; set; }

  public ModeratorAssignmentCommand(Moderator remote, IList<MapNodeListItem> mapNodes) : base(remote.CommandChannel, "moderatorassignment")
  {
    Data = new ModeratorAssignmentPayload { Remote = remote, MapNodes = mapNodes };
  }
}
