using Microsoft.CodeAnalysis.CSharp.Syntax;
using OLabWebAPI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Data.Contracts
{
  public class SessionReport
  {
    public SessionReport()
    {
      Nodes = new List<NodeSession>();
      Counters = new List<CounterSession>();
    }

    public string SessionId { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public string UserName { get; set; }
    public string CheckSum { get; set; }

    public IList<NodeSession> Nodes { get; set; }
    public IList<CounterSession> Counters{ get; set; }
  }

  public class NodeSession
  {
    public uint NodeId { get; set; }
    public DateTime TimeStamp { get; set; }
    public string NodeName { get; set; }
    public IList<NodeResponse> Responses { get; set; }

  }

  public class CounterSession
  {
    public uint Id { get; set; }
    public string Name { get; set; }
    public string Value{ get; set; }
  }

  public class NodeResponse
  {
    public DateTime TimeStamp {  get; set; }
    public uint QuestionId { get; set; }
    public string QuestionName { get; set; }
    public string QuestionType { get; set; }
    public bool isCorrect { get; set; }
    public string QuestionStem { get; set; }
    public string ResponseText { get; set; }
  }
}
