using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("attach_files"), Index(nameof(Id))]
    public class AttachFiles
    {
        public Guid? Id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileData { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public ICollection<AttachFileRelation> AttachFileRelations { get; set; } = [];
    }
}