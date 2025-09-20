using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("files"), Index(nameof(Id))]
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? FileData { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
    }
}