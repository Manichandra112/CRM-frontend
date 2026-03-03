namespace CRM_Backend.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    

    [Table("modules")]
    public class Module
    {
        [Key]
        [Column("module_id")]
        public long ModuleId { get; set; }

        [Required]
        [Column("module_code")]
        [MaxLength(50)]
        public string ModuleCode { get; set; } = null!;

        [Required]
        [Column("module_name")]
        [MaxLength(150)]
        public string ModuleName { get; set; } = null!;

        [Column("active")]
        public bool Active { get; set; } = true;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }
}
