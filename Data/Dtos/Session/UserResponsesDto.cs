using OLab.Api.Model;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace OLab.Data.Dtos.Session;

public class UserResponsesDto
{
  public DateTime CreatedAt { get; set; }
  public string Response { get; set; }
  public uint Id { get; set; }
  public uint NodeId { get; set; }
  public uint QuestionId { get; set; }
  public uint QuestionName { get; set; }
}
