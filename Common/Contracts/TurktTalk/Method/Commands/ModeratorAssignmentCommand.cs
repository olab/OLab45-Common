using OLab.Common.Contracts.TurktTalk.BusinessObjects;
using OLab.Common.Contracts.TurktTalk.Contracts;
using System.Collections.Generic;

namespace OLab.Common.Contracts.TurktTalk.Method.Commands;

public class ModeratorAssignmentCommand : CommandMethod
{
  public ModeratorAssignmentPayload Data { get; set; }

  public Moderator Remote { get; set; }

  public ModeratorAssignmentCommand(Moderator remote, IList<MapNodeListItem> mapNodes) : base( remote.CommandChannel, "moderatorassignment" )
  {
    Data = new ModeratorAssignmentPayload { Remote = remote, MapNodes = mapNodes };
  }
}
