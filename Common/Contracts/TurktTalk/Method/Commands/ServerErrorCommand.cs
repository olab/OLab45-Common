using OLab.Api.TurkTalk.Methods;

namespace OLab.Api.TurkTalk.Commands
{
  public class ServerErrorCommand : CommandMethod
  {
    public string Data { get; set; }

    public ServerErrorCommand(string connectionId, string message) : base(connectionId, "servererror")
    {
      Data = message;
    }
  }
}
