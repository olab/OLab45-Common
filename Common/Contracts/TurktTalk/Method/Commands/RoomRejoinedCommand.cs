using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Api.TurkTalk.BusinessObjects;
using OLab.Api.TurkTalk.Methods;

namespace OLab.Api.TurkTalk.Commands;

/// <summary>
/// Defines a Room Rejoined command method
/// </summary>
public class RoomRejoinedCommand : CommandMethod
{
  public Participant Data { get; set; }

  public RoomRejoinedCommand(string recipientGroupName, Participant participant) : base(recipientGroupName, "roomrejoined")
  {
    Data = participant;
  }

  public override string ToJson()
  {
    var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
    return JToken.Parse(rawJson).ToString(Formatting.Indented);
  }

}