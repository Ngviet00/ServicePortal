using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.TypeLeave.DTO;
using ServicePortal.Modules.TypeLeave.DTO.Requests;
using ServicePortal.Modules.TypeLeave.Services.Interfaces;

namespace ServicePortal.Modules.TypeLeave.Services
{
    public class TypeLeaveService : ITypeLeaveService
    {
        private readonly ApplicationDbContext _context;

        public TypeLeaveService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequestDto request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.TypeLeaves.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name != null && r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var typeLeaves = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Domain.Entities.TypeLeave>
            {
                Data = typeLeaves,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<Domain.Entities.TypeLeave> GetById(int id)
        {
            var result = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Type leave not found!");

            return result;
        }

        public async Task<Domain.Entities.TypeLeave> Create(TypeLeaveDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new ValidationException("Name can not empty!");
            }

            var typeLeave = new Domain.Entities.TypeLeave
            {
                Name = dto.Name,
                ModifiedBy = dto.ModifiedBy,
                ModifiedAt = DateTime.UtcNow,
            };

            _context.TypeLeaves.Add(typeLeave);

            await _context.SaveChangesAsync();

            return typeLeave;
        }

        public async Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDto dto)
        {
            var typeLeave = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Type leave not found!");

            typeLeave.Id = id;
            typeLeave.Name = dto.Name;
            typeLeave.ModifiedBy = dto.ModifiedBy;
            typeLeave.ModifiedAt = DateTime.UtcNow;

            _context.TypeLeaves.Update(typeLeave);

            await _context.SaveChangesAsync();

            return typeLeave;
        }

        public async Task<Domain.Entities.TypeLeave> Delete(int id)
        {
            var typeLeave = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Type leave not found!");

            _context.TypeLeaves.Remove(typeLeave);

            await _context.SaveChangesAsync();

            return typeLeave;
        }
    }
}
