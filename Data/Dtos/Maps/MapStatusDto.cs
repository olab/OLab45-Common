using System;
using System.Collections.Generic;

namespace OLab.Api.Dto;

public class ScopeObjectCount
{
  public int Questions { get; set; }
  public int QuestionRepsonses { get; set; }
  public int Constants { get; set; }
  public int Counters { get; set; }
  public int CounterActions { get; set; }
  public int Files { get; set; }

  public static ScopeObjectCount operator +(ScopeObjectCount a, ScopeObjectCount b)
  {
    var t = new ScopeObjectCount();

    t.Questions = a.Questions + b.Questions;
    t.QuestionRepsonses = a.QuestionRepsonses + b.QuestionRepsonses;
    t.Constants = a.Constants + b.Constants;
    t.Counters = a.Counters + b.Counters;
    t.CounterActions = a.CounterActions + b.CounterActions;
    t.Files = a.Files + b.Files;

    return t;
  }
}

public class MapStatusDto
{
  public uint Id { get; set; }
  public string Name { get; set; }
  public int NodeCount { get; set; }
  public int NodeLinkCount { get; set; }
  public string Author { get; set; }
  public string Abstract { get; set; }
  public IList<string> Groups { get; set; }
  public DateTime CreatedAt { get; set; }
  public ScopeObjectCount? Total { get; set; }
  public ScopeObjectCount? Server { get; set; }
  public ScopeObjectCount? Map { get; set; }
  public ScopeObjectCount? Node { get; set; }

  public MapStatusDto()
  {
  }

  public void UpdateTotal()
  {
    Total = new ScopeObjectCount();

    Total = Server + Total;
    Total = Map + Total;
    Total = Node + Total;
  }
}
