using ServicePortal.Modules.Role.Interfaces;
using ServicePortal.Infrastructure.Data;
using AutoMapper;
using ServicePortal.Common;
using Microsoft.EntityFrameworkCore;

namespace ServicePortal.Modules.Role.Services
{
    public class RoleService : IRoleService
    {
        private readonly ApplicationDbContext _context;

        public RoleService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.Role>> GetAll()
        {
            List<Domain.Entities.Role> roles = await _context.Roles.ToListAsync();

            return roles;
        }

        public async Task<Domain.Entities.Role> GetById(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Role not found!");

            return role;
        }

        public async Task<Domain.Entities.Role> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ValidationException("Name can not empty!");
            }

            var role = new Domain.Entities.Role { Name = name };

            _context.Roles.Add(role);

            await _context.SaveChangesAsync();

            return role;
        }

        public async Task<Domain.Entities.Role> Update(int id, string name)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Role not found!");

            role.Name = name;

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
