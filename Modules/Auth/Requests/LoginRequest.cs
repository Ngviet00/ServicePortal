﻿using System.ComponentModel.DataAnnotations;

namespace ServicePortal.Application.DTOs.Auth.Requests
{
    public class LoginRequest
    {
        [Required]
        public string? UserCode {  get; set; }
        [Required]
        public string? Password {  get; set; }
    }
}
