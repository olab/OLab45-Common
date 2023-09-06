namespace OLab.Api.Importer
{
  public class XmlMapNodeSectionNodes : XmlImportArray<XmlMapNodeSectionNode>
  {
  }

  public class XmlMapNodeSectionNode
  {
    public int Id;
    public int SectionId;
    public int NodeId;
    public int Order;
    public string NodeType;
  }
}