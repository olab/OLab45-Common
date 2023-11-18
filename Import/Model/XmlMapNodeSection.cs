namespace OLab.Api.Importer
{
  public class XmlMapNodeSections : XmlImportArray<XmlMapNodeSection>
  {
  }

  public class XmlMapNodeSection
  {
    public int Id;
    public string Name;
    public int MapId;
    public string OrderBy;
  }

}