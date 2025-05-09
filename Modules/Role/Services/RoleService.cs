using ServicePortal.Modules.Role.Interfaces;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Common;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Modules.Role.Requests;

namespace ServicePortal.Modules.Role.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Domain.Entities.Role>> GetAll(SearchRoleRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            query = query.OrderBy(e => e.Code);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var roles = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Domain.Entities.Role>
            {
                Data = roles,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<Domain.Entities.Role> GetById(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Role not found!");

            return role;
        }

        public async Task<Domain.Entities.Role> Create(CreateRoleRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ValidationException("Name can not empty!");
            }

            var role = new Domain.Entities.Role
            { 
                Name = request.Name,
                Code = request.Code
            };

            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Domain.Entities.Role> Update(int id, CreateRoleRequest request)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Role not found!");

            role.Name = request.Name;
            role.Code = request.Code;

            _context.Roles.Update(role);

            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Domain.Entities.Role> Delete(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Role not found!");

            _context.Roles.Remove(role);

            await _context.SaveChangesAsync();

            return role;
        }
    }
}
