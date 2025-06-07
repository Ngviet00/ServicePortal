using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("attach_file_relations"), Index(nameof(AttachFileId), nameof(RefId), nameof(RefType))]
    public class AttachFileRelation
    {
        public Guid? Id { get; set; }
        public Guid? AttachFileId { get; set; }
        public Guid? RefId { get; set; }
        public string? RefType { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public AttachFiles? AttachFiles { get; set; }
    }
}
