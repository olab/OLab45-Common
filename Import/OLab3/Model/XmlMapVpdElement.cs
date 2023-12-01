namespace OLab.Import.OLab3.Model
{
  public class XmlMapVpdElements : XmlImportArray<XmlMapVpdElement>
  {
  }

  public class XmlMapVpdElement
  {
    public uint Id;
    public uint VpdId;
    public string Key;
    public string Value;
  }
}