﻿using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortals.Domain.Entities
{
    [Table("user_configs"), Index(nameof(UserCode))]
    public class UserConfig
    {
        public Guid Id { get; set; }
        public string? UserCode { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
        public User? User { get; set; }
    }
}
