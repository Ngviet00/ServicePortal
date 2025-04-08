using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Application.DTOs.Auth.Requests
{
    public class LoginRequest
    {
        [Required]
        public string? EmployeeCode {  get; set; }
        [Required]
        public string? Password {  get; set; }
    }
}
