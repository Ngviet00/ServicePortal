using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("hr_managements")]
    public class HrManagements
    {
        public int? Id { get; set; }
        public string? Type {  get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
    }
}
