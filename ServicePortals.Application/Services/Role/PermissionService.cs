using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Role.Requests;
using ServicePortals.Application.Interfaces.Role;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.Role
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Permission>> GetAll(SearchRoleRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Permissions.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name != null && r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var permissions = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Permission>
            {
                Data = permissions,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }
    }
}
