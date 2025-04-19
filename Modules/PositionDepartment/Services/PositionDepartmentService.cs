using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.PositionDeparment.DTO;
using ServicePortal.Modules.PositionDeparment.Interfaces;

namespace ServicePortal.Modules.PositionDeparment.Services
{
    public class PositionDepartmentService : IPositionDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public PositionDepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PositionDepartmentDTO> Create(PositionDepartmentDTO dto)
        {
            var entity = PositionDepartmentMapper.ToEntity(dto);

            _context.PositionDepartments.Add(entity);

            await _context.SaveChangesAsync();

            return PositionDepartmentMapper.ToDto(entity);
        }
    }
}
