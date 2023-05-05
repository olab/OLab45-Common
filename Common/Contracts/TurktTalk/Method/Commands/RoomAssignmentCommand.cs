using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLabWebAPI.Common.Contracts;
using OLabWebAPI.TurkTalk.BusinessObjects;
using OLabWebAPI.TurkTalk.Methods;

namespace OLabWebAPI.TurkTalk.Commands
{
  /// <summary>
  /// Defines a Room Assignment command method
  /// </summary>
  public class RoomAssignmentCommand : CommandMethod
  {
    public RoomAssignmentPayload Data { get; set; }

    public RoomAssignmentCommand(Learner local, Moderator remote = null) :
          base(local == null ? remote.CommandChannel : local.CommandChannel, "roomassignment")
    {
      Data = new RoomAssignmentPayload { Local = local, Remote = remote, SlotIndex = local.SlotIndex };
    }

    public override string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }

  }
}