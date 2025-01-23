using System;

namespace OLab.Common.Attributes;

[AttributeUsage( AttributeTargets.Class )]
public class OLabModuleAttribute : Attribute
{
  private string _name;

  public OLabModuleAttribute(string name)
  {
    _name = name;
  }

  public string Name
  {
    get { return _name; }
    set { _name = value; }
  }
}
