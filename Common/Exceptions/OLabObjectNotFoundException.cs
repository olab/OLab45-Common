using System;

namespace OLabWebAPI.Common.Exceptions
{
  public class OLabObjectNotFoundException : Exception
  {
    public OLabObjectNotFoundException(string type, uint id) : base($"{type} object with id {id} not found")
    {
    }

    public OLabObjectNotFoundException(string type, string id) : base($"{type} object with id {id} not found")
    {
    }
  }
}