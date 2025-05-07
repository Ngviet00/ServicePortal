using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
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

            var query = _context.Users.Include(e => e.Department).AsQueryable();

            query = query.Where(e => e.Code != "0");

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.Contains(name) || u.Email.Contains(name) || u.Phone.Contains(name) || u.Code.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var usersWithDetails = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var result = new PagedResults<UserDTO>
            {
                Data = UserMapper.ToDtoList(usersWithDetails),
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

        public IQueryable<UserDTO> GetUserQueryLogin()
        {
            var query = _context.Users
                .Where(u => u.DeletedAt == null)
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Code = u.Code,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password,
                    IsActive = u.IsActive,
                    DateJoinCompany = u.DateJoinCompany,
                    Phone = u.Phone,
                    Sex = u.Sex,
                    Position = u.Position,
                    Level = u.Level,
                    LevelParent = u.LevelParent,
                    DepartmentId = u.DepartmentId,
                    Department = u.Department == null ? null : new DepartmentDTO
                    {
                        Id = u.Department.Id,
                        Name = u.Department.Name,
                    },

                    Roles = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => new Domain.Entities.Role
                        {
                            Id = ur.Role.Id,
                            Name = ur.Role.Name
                        })
                        .Distinct()
                        .ToList(),

                    UserPermissions = u.UserPermission
                        .Where(up => up.Permission != null)
                        .Select(up => up.Permission.Name)
                        .Distinct()
                        .ToList(),

                    Permissions = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .SelectMany(ur => ur.Role.RolePermissions
                            .Where(rp => rp.Permission != null)
                            .Select(rp => rp.Permission.Name)
                        )
                        .Distinct()
                        .ToList()
                });

            return query;
        }

        public async Task<long> CountUser()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<UserDTO> GetMe(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ValidationException("Code can not empty");
            }

            var user = await GetUserQueryLogin().FirstOrDefaultAsync(e => e.Code == code);

            if (user == null)
            {
                throw new NotFoundException("User not found!");
            }

            return user;
        }

        public async Task<List<OrgChartChildNode>> BuildTree(int? departmentId)
        {
            var users = await _context.Users
                .Where(e => e.DepartmentId == departmentId && e.Level != "0")
                .Select(x => new OrgChartUserDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Position = x.Position,
                    Level = x.Level,
                    LevelParent = x.LevelParent
                })
                .ToListAsync();

            var allLevelCodes = users.Select(x => x.Level).ToHashSet();

            var rootNodes = users
                .Where(x => string.IsNullOrEmpty(x.LevelParent) || !allLevelCodes.Contains(x.LevelParent))
                .Select(x => x.LevelParent)
                .Distinct();

            var trees = new List<OrgChartChildNode>();

            foreach (var root in rootNodes)
            {
                trees.AddRange(BuildTreeRecursive(root ?? "", users));
            }

            return trees;
        }

        private List<OrgChartChildNode> BuildTreeRecursive(string parentLevelCode, List<OrgChartUserDto> users)
        {
            var groupedChildren = users
                .Where(x => x.LevelParent == parentLevelCode)
                .GroupBy(x => x.Level);

            var children = new List<OrgChartChildNode>();

            foreach (var group in groupedChildren)
            {
                var levelCode = group.Key;

                var childNode = new OrgChartChildNode
                {
                    Level = levelCode ?? "",
                    People = group.ToList(),
                    Children = BuildTreeRecursive(levelCode ?? "", users)
                };

                children.Add(childNode);
            }

            return children;
        }
    }
}
