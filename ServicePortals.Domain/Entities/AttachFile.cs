using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("attach_files")]
    public class AttachFile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public int FileId { get; set; }
        public string? EntityType { get; set; }
        public long EntityId { get; set; }
        public File? File { get; set; }
    }
}
