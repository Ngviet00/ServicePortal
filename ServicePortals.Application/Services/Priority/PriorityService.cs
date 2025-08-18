using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Priority.Requests;
using ServicePortals.Application.Interfaces.Priority;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Priority
{
    public class PriorityService : IPriorityService
    {
        private readonly ApplicationDbContext _context;
        
        public PriorityService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Create(CreatePriorityRequest request)
        {
            if (await _context.Priorities.FirstOrDefaultAsync(e => e.Name == request.Name) != null)
            {
                throw new ValidationException("Priority has been exists");
            }

            var newItem = new Domain.Entities.Priority
            {
                Name = request.Name,
                NameE = request.NameE
            };

            _context.Priorities.Add(newItem);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Delete(int Id)
        {
            var item = await _context.Priorities.FirstOrDefaultAsync(e => e.Id == Id) ?? throw new NotFoundException("Priority not found");

            _context.Priorities.Remove(item);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<Domain.Entities.Priority>> GetAll(GetAllPriorityRequest request)
        {
            return await _context.Priorities.ToListAsync();
        }

        public async Task<Domain.Entities.Priority?> GetById(int Id)
        {
            return await _context.Priorities.FirstOrDefaultAsync(e => e.Id == Id);
        }

        public async Task<object> Update(int Id, UpdatePriorityRequest request)
        {
            var item = await GetById(Id);

            if (item != null)
            {
                item.Name = request.Name;
                item.NameE = request.NameE;

                _context.Priorities.Update(item);

                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
