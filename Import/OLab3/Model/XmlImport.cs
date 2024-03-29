
using System.Collections.Generic;

namespace OLab.Import.OLab3.Model;

public abstract class XmlImportObject
{
}

public class XmlImportArray<T> : XmlImportObject
{
  public IList<T> Data = new List<T>();
}