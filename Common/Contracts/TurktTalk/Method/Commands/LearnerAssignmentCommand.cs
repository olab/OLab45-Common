using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Common.Contracts;
using OLab.TurkTalk.BusinessObjects;
using OLab.TurkTalk.Methods;
using System.Collections.Generic;

namespace OLab.TurkTalk.Commands
{

  public class LearnerAssignmentPayload
  {
    public Participant Learner { get; set; }
    public int SlotIndex { get; set; }
    public IList<MapNodeListItem> JumpNodes { get; set; }

    public LearnerAssignmentPayload()
    {
      JumpNodes = new List<MapNodeListItem>();
    }
  }

  /// <summary>
  /// Defines a Learner Assignment command method
  /// </summary>
  public class LearnerAssignmentCommand : CommandMethod
  {
    public LearnerAssignmentPayload Data { get; set; }

    public LearnerAssignmentCommand(
      Participant moderator,
      Learner learner,
      IList<MapNodeListItem> jumpNodes) : base(moderator.CommandChannel, "learnerassignment")
    {
      Data = new LearnerAssignmentPayload
      {
        Learner = learner,
        SlotIndex = learner.SlotIndex,
        JumpNodes = jumpNodes
      };
    }

    public override string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }

  }
}