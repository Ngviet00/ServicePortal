using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.ITForm.Requests.ITCategory
{
    public class CreateITCategoryRequest
    {
        [Required]
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
}
