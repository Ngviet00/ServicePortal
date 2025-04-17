using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;
using ServicePortal.Modules.Deparment.Requests;

namespace ServicePortal.Modules.Deparment.Services
{
    public class DeparmentService : IDeparmentService
    {
        private readonly ApplicationDbContext _context;

        public DeparmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Domain.Entities.Deparment>> GetAll(GetAllDeparmentRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Deparments.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var deparments = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Domain.Entities.Deparment>
            {
                Data = deparments,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<Domain.Entities.Deparment> GetById(int id)
        {
            var deparment = await _context.Deparments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            return deparment;
        }

        public async Task<Domain.Entities.Deparment> Create(DeparmentDTO dto)
        {
            var deparment = DeparmentMapper.toEntity(dto);

            _context.Deparments.Add(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }

        public async Task<Domain.Entities.Deparment> Update(int id, DeparmentDTO dto)
        {
            var deparment = await _context.Deparments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            deparment = DeparmentMapper.toEntity(dto);

            _context.Deparments.Update(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }

        public async Task<Domain.Entities.Deparment> Delete(int id)
        {
            var deparment = await _context.Deparments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            _context.Deparments.Remove(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }
    }
}
