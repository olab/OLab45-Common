using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLabWebAPI.TurkTalk.BusinessObjects;
using OLabWebAPI.TurkTalk.Methods;
using System.Collections.Generic;
using System.Linq;

namespace OLabWebAPI.TurkTalk.Commands
{
  /// <summary>
  /// Defines a Atrium Update command method
  /// </summary>
  public class AtriumUpdateCommand : CommandMethod
  {
    public IList<Learner> Data { get; set; }

    // constructor for all moderators in a topic
    public AtriumUpdateCommand(string moderatorChannel, IList<Learner> atriumLearners) : base(moderatorChannel, "atriumupdate")
    {
      Data = atriumLearners.OrderBy(x => x.NickName).ToList();
    }

    public override string ToJson()
    {
      var rawJson = System.Text.Json.JsonSerializer.Serialize(this);
      return JToken.Parse(rawJson).ToString(Formatting.Indented);
    }

  }
}