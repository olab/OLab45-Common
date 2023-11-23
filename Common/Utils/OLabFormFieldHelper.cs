using OLab.Common.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace OLab.Common.Utils;
public class OLabFormFieldHelper : IOLabFormFieldHelper
{
  public IDictionary<string, object> Fields { get; private set; }
  public Stream Stream { get; set; }

  public OLabFormFieldHelper(Stream stream)
  {
    Fields = new Dictionary<string, object>();
    Stream = stream;
  }

  public string Field(string key)
  {
    if (Fields.ContainsKey(key))
      return Fields[key].ToString();

    return string.Empty;
  }
}
