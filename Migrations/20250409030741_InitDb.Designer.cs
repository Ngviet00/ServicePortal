﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServicePortal.Infrastructure.Data;

#nullable disable

namespace ServicePortal.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250409030741_InitDb")]
    partial class InitDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ServicePortal.Domain.Entities.ApprovalLeaveRequestStep", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<DateTime?>("ApprovedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("approved_at");

                    b.Property<string>("Deparment")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("deparment");

                    b.Property<Guid?>("LeaveRequestId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("leave_request_id");

                    b.Property<string>("Note")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("note");

                    b.Property<int?>("Order")
                        .HasColumnType("int")
                        .HasColumnName("order");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("position");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("status");

                    b.Property<string>("UserCodeApprover")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("user_code_approver");

                    b.HasKey("Id");

                    b.ToTable("approval_leave_request_steps");
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.Deparment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("name");

                    b.Property<string>("Note")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("note");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int")
                        .HasColumnName("parent_id");

                    b.HasKey("Id");

                    b.ToTable("deparments");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "IT/MIS",
                            Note = "IT"
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.LeaveRequest", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("deleted_at");

                    b.Property<bool?>("DisplayHr")
                        .HasColumnType("bit")
                        .HasColumnName("display_hr");

                    b.Property<DateTime?>("FromDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("from_date");

                    b.Property<string>("FromHour")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("from_hour");

                    b.Property<bool?>("HaveSalary")
                        .HasColumnType("bit")
                        .HasColumnName("have_salary");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("image");

                    b.Property<string>("MetaData")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("meta_data");

                    b.Property<string>("NameRegister")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name_register");

                    b.Property<string>("Reason")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("reason");

                    b.Property<string>("ReasonTypeLeaveOther")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("reason_type_leave_other");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("state");

                    b.Property<int?>("TimeLeave")
                        .HasColumnType("int")
                        .HasColumnName("time_leave");

                    b.Property<DateTime?>("ToDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("to_date");

                    b.Property<string>("ToHour")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("to_hour");

                    b.Property<int?>("TypeLeave")
                        .HasColumnType("int")
                        .HasColumnName("type_leave");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("updated_at");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("user_code");

                    b.HasKey("Id");

                    b.ToTable("leave_requests");
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("deleted_at");

                    b.Property<int?>("Level")
                        .HasColumnType("int")
                        .HasColumnName("level");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("positions");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Level = 0,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            Id = 1,
                            Level = 1,
                            Name = "General Director"
                        },
                        new
                        {
                            Id = 2,
                            Level = 2,
                            Name = "Assistant General Director"
                        },
                        new
                        {
                            Id = 3,
                            Level = 3,
                            Name = "General Manager"
                        },
                        new
                        {
                            Id = 4,
                            Level = 4,
                            Name = "Manager"
                        },
                        new
                        {
                            Id = 5,
                            Level = 5,
                            Name = "Assistant Manager"
                        },
                        new
                        {
                            Id = 6,
                            Level = 6,
                            Name = "Supervisor"
                        },
                        new
                        {
                            Id = 7,
                            Level = 6,
                            Name = "Chief Accountant"
                        },
                        new
                        {
                            Id = 8,
                            Level = 7,
                            Name = "Staff"
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("expires_at");

                    b.Property<bool?>("IsRevoked")
                        .HasColumnType("bit")
                        .HasColumnName("is_revoked");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("token");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("user_code");

                    b.HasKey("Id");

                    b.ToTable("refresh_tokens");
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "SuperAdmin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "IT"
                        },
                        new
                        {
                            Id = 3,
                            Name = "HR"
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("code");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("created_at");

                    b.Property<DateTime?>("DateJoinCompany")
                        .HasColumnType("datetime2")
                        .HasColumnName("date_join_company");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("deleted_at");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("bit")
                        .HasColumnName("is_active");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("password");

                    b.Property<int?>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("role_id");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("updated_at");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.UserAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<int?>("DeparmentId")
                        .HasColumnType("int")
                        .HasColumnName("deparment_id");

                    b.Property<bool?>("IsHeadOfDeparment")
                        .HasColumnType("bit")
                        .HasColumnName("is_head_of_deparment");

                    b.Property<int?>("PositionId")
                        .HasColumnType("int")
                        .HasColumnName("position_id");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("user_code");

                    b.HasKey("Id");

                    b.ToTable("user_assignments");
                });
#pragma warning restore 612, 618
        }
    }
}
