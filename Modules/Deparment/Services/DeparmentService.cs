using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;

namespace ServicePortal.Modules.Deparment.Services
{
    public class DeparmentService : IDeparmentService
    {
        private readonly ApplicationDbContext _context;

        public DeparmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Domain.Entities.Deparment>> GetAll()
        {
            List<Domain.Entities.Deparment> deparments = await _context.Deparments.ToListAsync();

            return deparments;
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
