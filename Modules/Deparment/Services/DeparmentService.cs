using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;

namespace ServicePortal.Modules.Deparment.Services
{
    public class DeparmentService : IDeparmentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DeparmentService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Domain.Entities.Deparment> Create(DeparmentDTO dto)
        {
            
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Deparment> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Deparment> ForceDelete(int id)
        {
            throw new NotImplementedException();
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

        public async Task<Domain.Entities.Deparment> Update(int id, DeparmentDTO dto)
        {
            var deparment = await _context.Deparments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            _context.Deparments.Add(_mapper.Map<Domain.Entities.Deparment>(dto));

            await _context.SaveChangesAsync();

            return deparment;
        }
    }
}
