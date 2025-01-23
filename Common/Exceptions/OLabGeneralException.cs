using System;

namespace OLab.Api.Common.Exceptions;

public class OLabGeneralException : Exception
{
  public OLabGeneralException(string message) : base( message )
  {
  }
}