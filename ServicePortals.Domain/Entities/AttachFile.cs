using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("attach_files")]
    [Index(nameof(FileId))]
    [Index(nameof(EntityType), nameof(EntityId))]
    public class AttachFile
    {
        public Guid? Id { get; set; }
        public Guid? FileId { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public File? File { get; set; }
    }
}
