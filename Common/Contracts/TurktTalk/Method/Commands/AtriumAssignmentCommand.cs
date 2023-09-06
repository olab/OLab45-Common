using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.TurkTalk.BusinessObjects;
using OLab.TurkTalk.Methods;

namespace OLab.TurkTalk.Commands
{
  /// <summary>
  /// Defines a Atrium Assignment command method
  /// </summary>
  public class AtriumAssignmentCommand : CommandMethod
  {
    public Learner Data { get; set; }

    public AtriumAssignmentCommand() : base()
    {

    }

    public AtriumAssignmentCommand(Participant participant, Learner atriumParticipant)
      : base(participant.CommandChannel, "atriumassignment")
    {
      Data = atriumParticipant;
    }

    public override string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }

  }
}