using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OLab.Common.Contracts.TurktTalk.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace OLab.Common.Contracts.TurktTalk.Method.Commands;

/// <summary>
/// Defines a Atrium Update command method
/// </summary>
public class AtriumUpdateCommand : CommandMethod
{
  public IList<Learner> Data { get; set; }

  // constructor for all moderators in a topic
  public AtriumUpdateCommand(string moderatorChannel, IList<Learner> atriumLearners) : base( moderatorChannel, "atriumupdate" )
  {
    Data = atriumLearners.OrderBy( x => x.NickName ).ToList();
  }

  public override string ToJson()
  {
    var rawJson = System.Text.Json.JsonSerializer.Serialize( this );
    return JToken.Parse( rawJson ).ToString( Formatting.Indented );
  }

}