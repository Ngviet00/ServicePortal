using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domain.Entities
{
    [Table("position_deparments")]
    public class PositionDeparment
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("deparment_id")]
        public int? DeparmentId { get; set; }

        [Column("position_id")]
        public int? PositionId { get; set; }

        [Column("position_deparment_level")]
        public int? PositionDeparmentLevel { get; set; }

        [Column("custom_title")]
        public string? CustomTitle { get; set; }
    }
}
