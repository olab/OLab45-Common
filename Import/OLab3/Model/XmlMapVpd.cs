namespace OLab.Import.OLab3.Model;

public class XmlMapVpds : XmlImportArray<XmlMapVpd>
{
}

public class XmlMapVpd
{
  public uint Id;
  public uint MapId;
  public uint VpdTypeId;
}