using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("q_cumulative")]
[Index("MapId", Name = "map_id")]
[Index("QuestionId", Name = "question_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class QCumulative
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("question_id")]
    public uint QuestionId { get; set; }

    [Column("map_id")]
    public uint MapId { get; set; }

    [Column("reset")]
    public int Reset { get; set; }

    [ForeignKey("MapId")]
    [InverseProperty("QCumulative")]
    public virtual Maps Map { get; set; }

    [ForeignKey("QuestionId")]
    [InverseProperty("QCumulative")]
    public virtual MapQuestions Question { get; set; }
}
