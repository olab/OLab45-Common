using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects.API
{
    [Table("h5p_counters")]
    public partial class H5pCounters
    {
        [Key]
        [Column("type")]
        [StringLength(63)]
        public string Type { get; set; }
        [Key]
        [Column("library_name")]
        [StringLength(127)]
        public string LibraryName { get; set; }
        [Key]
        [Column("library_version")]
        [StringLength(31)]
        public string LibraryVersion { get; set; }
        [Column("num", TypeName = "int(10) unsigned")]
        public uint Num { get; set; }
    }
}
