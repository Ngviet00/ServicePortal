using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("files"), Index(nameof(Id))]
    public class File
    {
        public Guid? Id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileData { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.Now;
    }
}