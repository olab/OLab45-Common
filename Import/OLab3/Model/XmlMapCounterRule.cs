namespace OLab.Import.OLab3.Model;

public class XmlMapCounterRules : XmlImportArray<XmlMapCounterRule>
{
}

public class XmlMapCounterRule
{
  public int Id;
  public int CounterId;
  public int RelationId;
  public int Value;
  public string Function;
  public int RedirectNodeId;
  public object Message;
  public int Counter;
  public string CounterValue;
}