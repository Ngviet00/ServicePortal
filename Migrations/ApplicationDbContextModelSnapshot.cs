﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ServicePortal.Infrastructure.Data;

#nullable disable

namespace ServicePortal.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.HasIndex("Id", "ParentId");

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

                    b.Property<int?>("DeparmentId")
                        .HasColumnType("int")
                        .HasColumnName("deparment_id");

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

                    b.Property<int?>("PositionId")
                        .HasColumnType("int")
                        .HasColumnName("position_id");

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

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<int?>("PositionLevel")
                        .HasColumnType("int")
                        .HasColumnName("position_level");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("positions");

                    b.HasData(
                        new
                        {
                            Id = -1,
                            Name = "SuperAdmin",
                            PositionLevel = 0
                        },
                        new
                        {
                            Id = 1,
                            Name = "General Director",
                            PositionLevel = 1
                        },
                        new
                        {
                            Id = 2,
                            Name = "Assistant General Director",
                            PositionLevel = 2
                        },
                        new
                        {
                            Id = 3,
                            Name = "General Manager",
                            PositionLevel = 3
                        },
                        new
                        {
                            Id = 4,
                            Name = "Manager",
                            PositionLevel = 4
                        },
                        new
                        {
                            Id = 5,
                            Name = "Assistant Manager",
                            PositionLevel = 5
                        },
                        new
                        {
                            Id = 6,
                            Name = "Supervisor",
                            PositionLevel = 6
                        },
                        new
                        {
                            Id = 7,
                            Name = "Chief Accountant",
                            PositionLevel = 6
                        },
                        new
                        {
                            Id = 8,
                            Name = "Staff",
                            PositionLevel = 7
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.PositionDeparment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CustomTitle")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("custom_title");

                    b.Property<int?>("DeparmentId")
                        .HasColumnType("int")
                        .HasColumnName("deparment_id");

                    b.Property<int?>("PositionDeparmentLevel")
                        .HasColumnType("int")
                        .HasColumnName("position_deparment_level");

                    b.Property<int?>("PositionId")
                        .HasColumnType("int")
                        .HasColumnName("position_id");

                    b.HasKey("Id");

                    b.HasIndex("DeparmentId", "PositionId", "PositionDeparmentLevel");

                    b.ToTable("position_deparments");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CustomTitle = "Manager IT/MIS",
                            DeparmentId = 1,
                            PositionDeparmentLevel = 1,
                            PositionId = 4
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<DateTimeOffset?>("CreatedAt")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("created_at");

                    b.Property<DateTimeOffset?>("ExpiresAt")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("expires_at");

                    b.Property<bool?>("IsRevoked")
                        .HasColumnType("bit")
                        .HasColumnName("is_revoked");

                    b.Property<string>("Token")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("token");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_code");

                    b.HasKey("Id");

                    b.HasIndex("Token", "UserCode", "ExpiresAt", "IsRevoked");

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

                    b.HasIndex("Id");

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
                        },
                        new
                        {
                            Id = 4,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<string>("Code")
                        .HasColumnType("nvarchar(450)")
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
                        .HasColumnType("nvarchar(450)")
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

                    b.HasIndex("Code", "Email", "Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("ServicePortal.Domain.Entities.UserAssignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("id");

                    b.Property<bool?>("IsHeadOfDeparment")
                        .HasColumnType("bit")
                        .HasColumnName("is_head_of_deparment");

                    b.Property<int?>("PositionDeparmentId")
                        .HasColumnType("int")
                        .HasColumnName("position_department_id");

                    b.Property<string>("UserCode")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("user_code");

                    b.HasKey("Id");

                    b.HasIndex("UserCode", "PositionDeparmentId");

                    b.ToTable("user_assignments");
                });
#pragma warning restore 612, 618
        }
    }
}
