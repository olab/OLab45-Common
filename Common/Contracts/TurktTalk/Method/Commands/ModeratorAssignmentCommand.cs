using OLab.Common.Contracts;
using OLab.TurkTalk.BusinessObjects;
using OLab.TurkTalk.Methods;
using System.Collections.Generic;

namespace OLab.TurkTalk.Commands
{
  public class ModeratorAssignmentCommand : CommandMethod
  {
    public ModeratorAssignmentPayload Data { get; set; }

    public Moderator Remote { get; set; }

    public ModeratorAssignmentCommand(Moderator remote, IList<MapNodeListItem> mapNodes) : base(remote.CommandChannel, "moderatorassignment")
    {
      Data = new ModeratorAssignmentPayload { Remote = remote, MapNodes = mapNodes };
    }
  }
}
