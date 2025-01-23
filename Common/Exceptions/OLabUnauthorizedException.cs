using System;

namespace OLab.Api.Common.Exceptions;

public class OLabUnauthorizedException : Exception
{
  public OLabUnauthorizedException(string type, uint id) : base( $"Unauthorized reading {type} object with id {id}" )
  {
  }

  public OLabUnauthorizedException(string message) : base( message )
  {
  }

  public OLabUnauthorizedException() : base( $"Unauthorized" )
  {
  }
}