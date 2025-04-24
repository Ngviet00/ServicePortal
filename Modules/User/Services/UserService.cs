using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Position.DTO;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.Interfaces;
using ServicePortal.Modules.User.Requests;

namespace ServicePortal.Modules.User.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<UserDTO>> GetAll(GetAllUserRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = GetUserQuery();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.Contains(name) || u.Email.Contains(name) || u.Phone.Contains(name));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var usersWithDetails = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var result = new PagedResults<UserDTO>
            {
                Data = usersWithDetails,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<UserDTO> GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("Code can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Code == code) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        public async Task<UserDTO> GetById(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            return UserMapper.ToDto(user);
        }

        //public async Task<UserDTO> Update(Guid id, UpdateUserRequest request)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

        //    throw new NotImplementedException();
        //}

        public async Task<UserDTO> Delete(Guid id)
        {
            if (string.IsNullOrWhiteSpace(id.ToString()))
            {
                throw new ValidationException("Id can not empty");
            }

            var user = await _context.Users.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("User not found!");

            user.DeletedAt = DateTime.Now;

            _context.Users.Update(user);

            await _context.SaveChangesAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<UserDTO> ForceDelete(Guid id)
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

        public IQueryable<UserDTO> GetUserQuery()
        {
            var query = _context.Users.AsQueryable();

            var userQuery = query
             .GroupJoin(_context.Roles, u => u.RoleId, r => r.Id, (u, r) => new { u, r })
             .SelectMany(temp => temp.r.DefaultIfEmpty(), (temp, r) => new { temp.u, Role = r })

             .GroupJoin(_context.Departments, temp => temp.u.ParentDepartmentId, d => d.Id, (temp, pd) => new { temp.u, temp.Role, pd })
             .SelectMany(temp => temp.pd.DefaultIfEmpty(), (temp, pd) => new { temp.u, temp.Role, ParentDepartment = pd })

             .GroupJoin(_context.Departments, temp => temp.u.ChildDepartmentId, d => d.Id, (temp, cd) => new { temp.u, temp.Role, temp.ParentDepartment, cd })
             .SelectMany(temp => temp.cd.DefaultIfEmpty(), (temp, cd) => new { temp.u, temp.Role, temp.ParentDepartment, ChildrenDepartment = cd })

             .GroupJoin(_context.Positions, temp => temp.u.PositionId, p => p.Id, (temp, pos) => new { temp.u, temp.Role, temp.ParentDepartment, temp.ChildrenDepartment, pos })
             .SelectMany(temp => temp.pos.DefaultIfEmpty(), (temp, position) => new UserDTO
             {
                 Code = temp.u.Code,
                 Name = temp.u.Name,
                 Email = temp.u.Email,
                 Password = temp.u.Password,
                 IsActive = temp.u.IsActive,
                 DateJoinCompany = temp.u.DateJoinCompany,
                 Phone = temp.u.Phone,
                 Sex = temp.u.Sex,
                 Role = temp.Role != null ? new Domain.Entities.Role
                 {
                     Id = temp.Role.Id,
                     Name = temp.Role.Name
                 } : null,
                 ParentDepartment = temp.ParentDepartment == null ? null : new DepartmentDTO
                 {
                     Id = temp.ParentDepartment.Id,
                     Name = temp.ParentDepartment.Name,
                     Note = temp.ParentDepartment.Note
                 },
                 ChildrenDepartment = temp.ChildrenDepartment == null ? null : new DepartmentDTO
                 {
                     Id = temp.ChildrenDepartment.Id,
                     Name = temp.ChildrenDepartment.Name,
                     Note = temp.ChildrenDepartment.Note
                 },
                 Position = position == null ? null : new PositionDTO
                 {
                     Id = position.Id,
                     Name = position.Name,
                     Title = position.Title,
                     Level = position.Level
                 }
             });

            return userQuery;
        }

        public async Task<long> CountUser()
        {
            return await _context.Users.CountAsync();
        }
    }
}
