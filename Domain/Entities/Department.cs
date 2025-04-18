﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Domain.Entities
{
    [Table("departments"), Index(nameof(Id), nameof(ParentId))]
    public class Department
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name"), MaxLength(50)]
        public string? Name { get; set; }

        [Column("note"), MaxLength(255)]
        public string? Note { get; set; }

        [Column("parent_id")]
        public int? ParentId { get; set; }

        public static implicit operator bool(Department? v)
        {
            throw new NotImplementedException();
        }
    }
}
