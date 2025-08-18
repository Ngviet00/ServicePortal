using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Role.Requests;
using ServicePortals.Application.Interfaces.Role;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Role
{
    /// <summary>
    /// CRUD permission
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Permission>> GetAll(SearchPermissionRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Permissions.AsQueryable();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var permissions = await query.OrderBy(e => e.Name).Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Permission>
            {
                Data = permissions,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<Permission> GetById(int id)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Permission not found!");

            return permission;
        }

        public async Task<Permission> Create(CreatePermissionRequest request)
        {
            var permission = new Permission
            {
                Name = request.Name,
                Group = request.Group
            };

            _context.Permissions.Add(permission);

            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission> Update(int id, CreatePermissionRequest request)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Permission not found!");

            permission.Name = request.Name;
            permission.Group = request.Group;

            _context.Permissions.Update(permission);

            await _context.SaveChangesAsync();

            return permission;
        }

        public async Task<Permission> Delete(int id)
        {
            var permission = await _context.Permissions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Permission not found!");

            _context.Permissions.Remove(permission);

            await _context.SaveChangesAsync();

            return permission;
        }
    }
}
