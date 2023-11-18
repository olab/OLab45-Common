
using System.Collections.Generic;

namespace OLab.Api.Importer
{
  public abstract class XmlImportObject
  {
  }

  public class XmlImportArray<T> : XmlImportObject
  {
    public IList<T> Data = new List<T>();
  }

}