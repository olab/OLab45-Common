using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("sjt_response")]
[Index("ResponseId", Name = "response_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class SjtResponse
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("response_id")]
    public uint ResponseId { get; set; }

    [Column("position")]
    public int Position { get; set; }

    [Column("points")]
    public int Points { get; set; }

    [ForeignKey("ResponseId")]
    [InverseProperty("SjtResponse")]
    public virtual MapQuestionResponses Response { get; set; }
}
