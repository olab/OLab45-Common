using System;

namespace OLab.Common.Exceptions
{
  public class OLabBadRequestException : Exception
  {
    public OLabBadRequestException(string message) : base(message)
    {
    }
  }
}