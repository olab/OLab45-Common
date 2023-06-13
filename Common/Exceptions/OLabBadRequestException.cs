using System;

namespace OLabWebAPI.Common.Exceptions
{
  public class OLabBadRequestException : Exception
  {
    public OLabBadRequestException(string message) : base(message)
    {
    }
  }
}