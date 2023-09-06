namespace OLab.Importer
{
  public class XmlMapVpds : XmlImportArray<XmlMapVpd>
  {
  }

  public class XmlMapVpd
  {
    public uint Id;
    public uint MapId;
    public uint VpdTypeId;
  }

}