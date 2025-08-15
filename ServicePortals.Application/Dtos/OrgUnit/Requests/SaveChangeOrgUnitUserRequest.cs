using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.OrgUnit.Requests
{
    public class SaveChangeOrgUnitUserRequest
    {
        [Required]
        public List<string> UserCodes { get; set; } = [];

        [Required]
        public int ViTriToChucId { get; set; }
    }
}
