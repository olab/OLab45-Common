using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Common.Contracts.TurktTalk.BusinessObjects;
using System.Collections.Generic;

namespace OLab.Common.Contracts.TurktTalk.Method.Commands;

/// <summary>
/// Defines a Learners Update command method
/// </summary>
public class LearnerListCommand : CommandMethod
{
  public IList<Learner> Data { get; set; }

  // constructor for targetted group
  public LearnerListCommand(string groupName, IList<Learner> atriumLearners) : base( groupName, "learnerlist" )
  {
    Data = atriumLearners;
  }

  public override string ToJson()
  {
    var rawJson = System.Text.Json.JsonSerializer.Serialize( this );
    return JToken.Parse( rawJson ).ToString( Formatting.Indented );
  }
}