using Dawn;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OLab.Api.TurkTalk.Methods;

/// <summary>
/// Defines a command method
/// </summary>
public class CommandMethod : Method
{
  public string Command { get; set; }

  public CommandMethod() : base()
  {

  }

  public CommandMethod(string recipientGroupName, string command) : base(recipientGroupName, "Command")
  {
    Guard.Argument(command).NotEmpty(command);
    Command = command;
  }

  public override string ToJson()
  {
    var rawJson = JsonConvert.SerializeObject(this);
    return JToken.Parse(rawJson).ToString(Formatting.Indented);
  }
}