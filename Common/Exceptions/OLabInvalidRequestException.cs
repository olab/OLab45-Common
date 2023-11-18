using System;
using System.Diagnostics.CodeAnalysis;

namespace OLab.Api.Common.Exceptions
{
  [ExcludeFromCodeCoverage]
  public class OLabInvalidRequestException : Exception
  {
    public OLabInvalidRequestException(Exception exception)
        : base("Invalid request", exception)
    { }

    public OLabInvalidRequestException(string message)
        : base(message)
    { }
  }
}