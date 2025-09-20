using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //trạng thái - pending, in process, complete, reject,...
    [Table("request_statuses")]
    public class RequestStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? Name { get; set; } //tiếng việt

        [MaxLength(50)]
        public string? NameE { get; set; } //tiếng anh
    }
}
