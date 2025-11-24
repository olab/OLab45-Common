using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OLab.Api.Model;

[Table("map_chat_elements")]
[Index("ChatId", Name = "chat_id")]
[MySqlCharSet("utf8mb3")]
[MySqlCollation("utf8mb3_general_ci")]
public partial class MapChatElements
{
    [Key]
    [Column("id")]
    public uint Id { get; set; }

    [Column("chat_id")]
    public uint ChatId { get; set; }

    [Required]
    [Column("question", TypeName = "text")]
    public string Question { get; set; }

    [Required]
    [Column("response", TypeName = "text")]
    public string Response { get; set; }

    [Required]
    [Column("function")]
    [StringLength(10)]
    public string Function { get; set; }

    [ForeignKey("ChatId")]
    [InverseProperty("MapChatElements")]
    public virtual MapChats Chat { get; set; }
}
