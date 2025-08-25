using System.ComponentModel.DataAnnotations;

namespace ServicePortals.Application.Dtos.Auth.Requests
{
    public class CreateUserRequest
    {
        [Required]
        public string? UserCode { get; set; }
    }
}