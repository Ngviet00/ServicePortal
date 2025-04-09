using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Position.Interfaces;

namespace ServicePortal.Modules.Position.Services
{
    public class PositionService : IPositionService
    {
        private readonly ApplicationDbContext _context;

        public PositionService(ApplicationDbContext context)
        { 
            _context = context;
        }

        public async Task<Domain.Entities.Position> Create(string name, int level)
        {
            var position = new Domain.Entities.Position
            {
                Name = name,
                Level = level
            };

            _context.Positions.Add(position);

            await _context.SaveChangesAsync();

            return position;
        }

        public async Task<List<Domain.Entities.Position>> GetAll()
        {
            List<Domain.Entities.Position> positions = await _context.Positions.ToListAsync();

            return positions;
        }

        public async Task<Domain.Entities.Position> GetById(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            return position;
        }

        public async Task<Domain.Entities.Position> Update(int id, string name, int level)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            position.Name = name;

            position.Level = level;

            _context.Positions.Update(position);

            await _context.SaveChangesAsync();

            return position;
        }

        public async Task<Domain.Entities.Position> Delete(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            position.DeletedAt = DateTime.Now;

            _context.Positions.Update(position);

            await _context.SaveChangesAsync();

            return position;
        }

        public async Task<Domain.Entities.Position> ForceDelete(int id)
        {
            var position = await _context.Positions.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Position not found!");

            _context.Positions.Remove(position);

            await _context.SaveChangesAsync();

            return position;
        }
    }
}
