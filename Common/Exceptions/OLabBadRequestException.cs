using System;

namespace OLab.Api.Common.Exceptions;

public class OLabBadRequestException : Exception
{
  public OLabBadRequestException(string message) : base(message)
  {
  }
}