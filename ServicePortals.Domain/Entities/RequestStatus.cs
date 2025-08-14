using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    //trạng thái - pending, in process, complete, reject,...
    [Table("request_statuses")]
    public class RequestStatus
    {
        public int? Id { get; set; }
        public string? Name { get; set; } //tiếng việt
        public string? NameE { get; set; } //tiếng anh
    }
}
