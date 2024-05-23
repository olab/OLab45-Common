using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Dto;

public partial class MapGroupsDto
{
  public uint Id { get; set; }
  public uint MapId { get; set; }
  public uint GroupId { get; set; }
}
