using System;

namespace OLab.Api.Data.Exceptions;

public class OLabObjectNotFoundException : Exception
{
  public OLabObjectNotFoundException(string type, uint id) : base( $"{type} object with id {id} not found" )
  {
    Type = type;
    Id = id;
  }

  public OLabObjectNotFoundException(string type, string id) : base( $"{type} object with id {id} not found" )
  {
  }

  public string Type { get; }
  public uint Id { get; }
}