using OLabWebAPI.TurkTalk.BusinessObjects;
using OLabWebAPI.Common.Contracts;
using System.Collections.Generic;
using OLabWebAPI.TurkTalk.Methods;

namespace OLabWebAPI.TurkTalk.Commands
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
