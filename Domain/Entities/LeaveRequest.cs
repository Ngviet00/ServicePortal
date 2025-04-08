﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ServicePortal.Domains.Models
{
    [Table("leave_requests")]
    public class LeaveRequest
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("user_code")]
        public string? UserCode { get; set; }

        [Column("name_register")]
        public string? NameRegister { get; set; }

        [Column("reason")]
        public string? Reason { get; set; }

        [Column("from_hour")]
        public string? FromHour { get; set; }

        [Column("from_date")]
        public DateTime? FromDate { get; set; }

        [Column("to_hour")]
        public string? ToHour { get; set; }

        [Column("to_date")]
        public DateTime? ToDate { get; set; }

        [Column("state")]
        public string? Status { get; set; } //statut total of request //pending, complete, reject

        [Column("display_hr")]
        public bool? DisplayHr { get; set; }

        [Column("type_leave")]
        public int? TypeLeave { get; set; }

        [Column("reason_type_leave_other")]
        public string? ReasonTypeLeaveOther { get; set; }

        [Column("time_leave")]
        public int? TimeLeave { get; set; }

        [Column("have_salary")]
        public bool? HaveSalary { get; set; }

        [Column("image")]
        public string? Image { get; set; }

        [Column("meta_data")]
        public string? MetaData { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }
}
