using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

public partial class MapGroups
{
  public MapGroups()
  {

  }
  public MapGroups(uint mapId, uint groupId)
  {
    MapId = mapId;
    GroupId = groupId;
  }
}
