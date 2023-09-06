using System;

namespace OLab.Common.Exceptions
{
  public class OLabGeneralException : Exception
  {
    public OLabGeneralException(string message) : base(message)
    {
    }
  }
}