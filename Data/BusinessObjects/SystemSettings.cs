using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace OLab.Data.BusinessObjects
{
    [Table("system_settings")]
    public partial class SystemSettings
    {
        [Key]
        [Column("id", TypeName = "int(10) unsigned")]
        public uint Id { get; set; }
        [Required]
        [Column("key")]
        [StringLength(45)]
        public string Key { get; set; }
        [Column("description")]
        [StringLength(256)]
        public string Description { get; set; }
        [Required]
        [Column("value")]
        [StringLength(256)]
        public string Value { get; set; }
        [Column("system_settingscol")]
        [StringLength(45)]
        public string SystemSettingscol { get; set; }
    }
}
