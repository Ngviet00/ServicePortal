using Hangfire;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ServicePortal.Applications.Modules.User.DTO.Requests;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Applications.Modules.User.Services.Interfaces;
using ServicePortal.Common;
using ServicePortal.Common.Helpers;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Infrastructure.Hubs;

namespace ServicePortal.Applications.Modules.User.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        private readonly NotificationService _notificationService;

        private readonly OrgChartBuilder _orgChartBuilder;

        public UserService(ApplicationDbContext context, NotificationService notificationService, OrgChartBuilder orgChartBuilder)
        {
            _context = context;
            _notificationService = notificationService;
            _orgChartBuilder = orgChartBuilder;
        }

        public async Task<PagedResults<UserResponseDto>> GetAll(GetAllUserRequestDto request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Users
                .Include(e => e.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            query = query.Where(e => e.UserCode != "0");

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.UserCode != null && u.UserCode.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var usersWithDetails = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var result = new PagedResults<UserResponseDto>
            {
                Data = UserMapper.ToDtoList(usersWithDetails),
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<UserResponseDto> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("Code can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == code) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponseDto> GetById(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponseDto> Delete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<UserResponseDto> ForceDelete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public IQueryable<UserResponseDto> GetUserQueryLogin()
        {
            //var query = _context.Users
            //    .Where(u => u.DeletedAt == null)
            //    .Select(u => new UserResponseDto
            //    {
            //        Id = u.Id,
            //        Code = u.Code,
            //        Name = u.Name,
            //        Email = u.Email,
            //        Password = u.Password,
            //        IsActive = u.IsActive,
            //        DateJoinCompany = u.DateJoinCompany,
            //        Phone = u.Phone,
            //        Sex = u.Sex,
            //        Position = u.Position,
            //        Level = u.Level,
            //        LevelParent = u.LevelParent,
            //        DepartmentId = u.DepartmentId,
            //        Department = u.Department == null ? null : new DepartmentDTO
            //        {
            //            Id = u.Department.Id,
            //            Name = u.Department.Name,
            //        },

            //        Roles = u.UserRoles
            //            .Where(ur => ur.Role != null)
            //            .Select(ur => new Domain.Entities.Role
            //            {
            //                Id = ur.Role!.Id,
            //                Name = ur.Role.Name,
            //                Code = ur.Role.Code
            //            })
            //            .Distinct()
            //            .ToList(),
            //        UserPermissions = new List<string?>(),

            //        //UserPermissions = u.UserPermission
            //        //    .Where(up => up.Permission != null)
            //        //    .Select(up => up.Permission!.Name)
            //        //    .Distinct()
            //        //    .ToList(),
            //        Permissions = new List<string?>(),

            //        //Permissions = u.UserRoles
            //        //    .Where(ur => ur.Role != null)
            //        //    .SelectMany(ur => ur.Role!.RolePermissions
            //        //        .Where(rp => rp.Permission != null)
            //        //        .Select(rp => rp.Permission!.Name)
            //        //    )
            //        //    .Distinct()
            //        //    .ToList()
            //    });

            //return query;
            return null;
        }

        public async Task<long> CountUser()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<object> GetMe(string code)
        {
            code = "22757";

            return null;
        }

        public async Task<bool> UpdateUserRole(UpdateUserRoleDto dto)
        {
            try
            {
                List<UserRole> ur = new List<UserRole>();

                var oldUserRoles = await _context.UserRoles.Where(e => e.UserCode == dto.UserCode).ToListAsync();
                _context.UserRoles.RemoveRange(oldUserRoles);

                if (dto?.RoleIds != null)
                {
                    foreach (var item in dto.RoleIds)
                    {
                        ur.Add(new UserRole
                        {
                            UserCode = dto.UserCode,
                            RoleId = item
                        });
                    }
                }

                _context.UserRoles.AddRange(ur);

                await _context.SaveChangesAsync();

                await _notificationService.SendMessageToUser(dto?.UserCode ?? "", "login_again");

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error can not update user role, error: {ex.Message}");
                return false;
            }
        }

        public async Task<UserResponseDto> ResetPassword(ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.UserCode == request.UserCode) ?? throw new NotFoundException("User not found!");

            var password = !string.IsNullOrWhiteSpace(request.Password) ? request.Password : request.UserCode ?? "123456";

            user.Password = Helper.HashString(password);

            user.IsChangePassword = 0;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            //check email exist
            BackgroundJob.Enqueue<EmailService>(job => job.SendEmailResetPassword("nguyenviet@vsvn.com.vn", password));

            return UserMapper.ToDto(user);
        }

        public async Task<OrgChartNode> BuildTree(int? departmentId)
        {
            return await _orgChartBuilder.BuildTree(departmentId);
        }
    }
}
