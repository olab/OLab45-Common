using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("links_order")]
[MySqlCollation("utf8mb4_0900_bin")]
public partial class LinksOrder
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("node_id_1")]
    public int? NodeId1 { get; set; }

    [Column("node_id_2")]
    public int? NodeId2 { get; set; }

    [Column("node_x")]
    public double? NodeX { get; set; }

    [Column("node_y")]
    public double? NodeY { get; set; }

    [Column("link_order")]
    public int? LinkOrder { get; set; }
}
