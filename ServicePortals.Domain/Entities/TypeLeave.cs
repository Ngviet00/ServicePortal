using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortals.Domain.Entities
{
    [Table("type_leaves")]
    public class TypeLeave
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Note { get; set; }
        public string? NameV {  get; set; }
        public string? Code { get; set; }
        public string? BgColor { get; set; }
        public string? TextColor { get; set; }
    }
}
