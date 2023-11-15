using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Api.Model;

public partial class UserSessionTracesDto
{
  public bool IsRedirected { get; set; }
  public decimal? BookmarkMade { get; set; }
  public decimal? BookmarkUsed { get; set; }
  public decimal? DateStamp { get; set; }
  public decimal? EndDateStamp { get; set; }
  public short? Confidence { get; set; }
  public string Counters { get; set; }
  public string Dams { get; set; }
  public uint Id { get; set; }
  public uint MapId { get; set; }
  public uint NodeId { get; set; }
  public uint SessionId { get; set; }
  public uint UserId { get; set; }
}
