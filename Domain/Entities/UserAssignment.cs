using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("user_assignments"), Index(nameof(UserCode), nameof(PositionDeparmentId))]
    public class UserAssignment
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("position_department_id")]
        public int? PositionDeparmentId { get; set; }

        [Column("is_head_of_deparment")]
        public bool? IsHeadOfDeparment { get; set; }
    }
}
