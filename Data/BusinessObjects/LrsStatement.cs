using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("lrs_statement")]
[Index("LrsId", Name = "lrs_id")]
[Index("StatementId", Name = "statement_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class LrsStatement
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("lrs_id")]
    public uint LrsId { get; set; }

    [Column("statement_id")]
    public uint StatementId { get; set; }

    [Column("status")]
    public byte Status { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at", TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [ForeignKey("LrsId")]
    [InverseProperty("LrsStatement")]
    public virtual Lrs Lrs { get; set; }

    [ForeignKey("StatementId")]
    [InverseProperty("LrsStatement")]
    public virtual Statements Statement { get; set; }
}
