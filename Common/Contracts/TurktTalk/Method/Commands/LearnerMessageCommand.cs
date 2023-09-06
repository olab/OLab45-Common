using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Common.Contracts;
using OLab.TurkTalk.Methods;

namespace OLab.TurkTalk.Commands
{
  public class LearnerMessageCommand : Method
  {
    public string Data { get; set; }
    /// <summary>
    /// Defines a Moderator removed command method
    /// </summary>
    public LearnerMessageCommand(MessagePayload payload) : base(payload.Envelope.To, "TURKEEMESSAGE")
    {
      Data = payload.Data;
    }

    public override string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }

  }
}