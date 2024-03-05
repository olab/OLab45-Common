using OLab.Api.Model;

namespace OLab.Import.OLab3.Model;

public class XmlMapNodeLinks : XmlImportArray<MapNodeLinks>
{
}

public class XmlMapNodeLink
{
  public int Id;
  public int MapId;
  public int NodeId1;
  public int NodeId2;
  public int ImageId;
  public string Text;
  public int Order;
  public int Probability;
  public int Hidden;
}