using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domains.Models
{
    [Table("user_assignments")]
    public class UserAssignment
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("deparment_id")]
        public int? DeparmentId { get; set; }

        [Column("position_id")]
        public int? PositionId { get; set; }

        [Column("is_head_of_deparment")]
        public bool? IsHeadOfDeparment { get; set; }
    }
}
