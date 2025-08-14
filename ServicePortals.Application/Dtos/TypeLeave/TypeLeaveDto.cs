using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.TypeLeave
{
    public class TypeLeaveDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? NameE { get; set; }
        [Required]
        public string? Code { get; set; }
    }
}
