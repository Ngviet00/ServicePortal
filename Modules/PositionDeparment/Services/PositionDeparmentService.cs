using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.PositionDeparment.DTO;
using ServicePortal.Modules.PositionDeparment.Interfaces;

namespace ServicePortal.Modules.PositionDeparment.Services
{
    public class PositionDeparmentService : IPositionDeparmentService
    {
        private readonly ApplicationDbContext _context;

        public PositionDeparmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PositionDeparmentDTO> Create(PositionDeparmentDTO dto)
        {
            var entity = PositionDeparmentMapper.ToEntity(dto);

            _context.PositionDeparments.Add(entity);

            await _context.SaveChangesAsync();

            return PositionDeparmentMapper.ToDto(entity);
        }
    }
}
