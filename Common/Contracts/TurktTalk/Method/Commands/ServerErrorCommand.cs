using OLab.TurkTalk.Methods;

namespace OLab.TurkTalk.Commands
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
