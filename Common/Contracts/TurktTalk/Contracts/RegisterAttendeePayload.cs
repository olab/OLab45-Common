using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OLab.Api.Common.Contracts
{
  public class RegisterAttendeePayload
  {
    public string ContextId { get; set; }
    public uint MapId { get; set; }
    public uint NodeId { get; set; }
    public uint QuestionId { get; set; }

    public string RoomName { get; set; }
    public string ReferringNode { get; set; }
    public string ConnectionId { get; set; }
    public string UserKey { get; set; }

    public string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }
  }
}
