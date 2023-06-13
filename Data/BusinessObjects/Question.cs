using Microsoft.EntityFrameworkCore;
using OLabWebAPI.ObjectMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLabWebAPI.Model
{
  [Table("Questions")]
  public abstract class Question
  {
    public Question()
    {
      InverseParent = new HashSet<Question>();
    }

    [Key]
    [Column("id", TypeName = "int(10) unsigned")]
    public uint Id { get; set; }
    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; }
    [Column("description", TypeName = "text")]
    public string Description { get; set; }
    [Column("parent_id", TypeName = "int(10) unsigned")]
    public uint? ParentId { get; set; }
    [Column("stem")]
    [StringLength(500)]
    public string Stem { get; set; }
    [Column("entry_type_id", TypeName = "int(10) unsigned")]
    public uint EntryTypeId { get; set; }
    [Column("width", TypeName = "int(10)")]
    public int Width { get; set; }
    [Column("height", TypeName = "int(10)")]
    public int Height { get; set; }
    [Column("feedback")]
    [StringLength(1000)]
    public string Feedback { get; set; }
    [Required]
    [Column("prompt", TypeName = "text")]
    public string Prompt { get; set; }
    [Column("show_answer")]
    public bool ShowAnswer { get; set; }
    [Column("counter_id", TypeName = "int(10)")]
    public uint? CounterId { get; set; }
    [Column("num_tries", TypeName = "int(10)")]
    public int NumTries { get; set; }
    [Column("show_submit", TypeName = "tinyint(4)")]
    public sbyte ShowSubmit { get; set; }
    [Column("redirect_node_id", TypeName = "int(10) unsigned")]
    public uint? RedirectNodeId { get; set; }
    [Column("submit_text")]
    [StringLength(200)]
    public string SubmitText { get; set; }
    [Column("type_display", TypeName = "int(10)")]
    public int TypeDisplay { get; set; }
    [Column("settings", TypeName = "text")]
    public string Settings { get; set; }
    [Column("is_private", TypeName = "int(4)")]
    public int IsPrivate { get; set; }
    [Column("order", TypeName = "int(10)")]
    public int? Order { get; set; }
    [Column("external_source_id")]
    [StringLength(255)]
    public string ExternalSourceId { get; set; }
    //[Required]
    //[Column("imageable_id", TypeName = "int(10) unsigned")]
    //public virtual uint ImageableId { get; set; }
    [Required]
    [Column("imageable_type")]
    [StringLength(45)]
    public string ImageableType { get; set; }
    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column("updated_At", TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    public virtual Question Parent { get; set; }
    [InverseProperty(nameof(Parent))]
    public virtual ICollection<Question> InverseParent { get; set; }
  }

  public class MapQuestion : Question
  {
    public Maps Map { get; set; } = null;

    [Required]
    [Column("imageable_id", TypeName = "int(10) unsigned")]
    public uint MapId { get; set; }
  }

  public class MapNodeQuestion : Question
  {
    public virtual MapNodes MapNode { get; set; }

    [Required]
    [Column("imageable_id", TypeName = "int(10) unsigned")]
    public uint MapNodeId { get; set; }
  }

  public class ServerQuestion : Question
  {
    public virtual Servers Server { get; set; }

    [Required]
    [Column("imageable_id", TypeName = "int(10) unsigned")]
    public uint ServerId { get; set; }
  }

}
