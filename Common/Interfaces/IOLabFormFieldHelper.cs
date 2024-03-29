using System.Collections.Generic;
using System.IO;

namespace OLab.Common.Interfaces;
public interface IOLabFormFieldHelper
{
  IDictionary<string, object> Fields { get; }
  Stream Stream { get; }
  string Field(string key);

}