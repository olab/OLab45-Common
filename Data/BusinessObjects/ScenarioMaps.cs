using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OLab.Api.Model;

[Table( "scenario_maps" )]
[Index( "MapId", Name = "fk_scenario_maps_maps_idx" )]
[Index( "ScenarioId", Name = "fk_scenerio_maps_idx" )]
[MySqlCharSet( "utf8mb3" )]
[MySqlCollation( "utf8mb3_general_ci" )]
public partial class ScenarioMaps
{
  [Key]
  [Column( "id", TypeName = "int(10) unsigned" )]
  public uint Id { get; set; }

  [Column( "map_id", TypeName = "int(10) unsigned" )]
  public uint MapId { get; set; }

  [Column( "scenario_id", TypeName = "int(10) unsigned" )]
  public uint ScenarioId { get; set; }

  [ForeignKey( "MapId" )]
  [InverseProperty( "ScenarioMaps" )]
  public virtual Maps Map { get; set; }

  [ForeignKey( "ScenarioId" )]
  [InverseProperty( "ScenarioMaps" )]
  public virtual Scenarios Scenario { get; set; }
}
