using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.RequestType.Request;
using ServicePortals.Application.Interfaces.RequestType;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.RequestType
{
    /// <summary>
    /// CRUD request type
    /// </summary>
    public class RequestTypeService : IRequestTypeService
    {
        private readonly ApplicationDbContext _context;

        public RequestTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Domain.Entities.RequestType>> GetAll(SearchRequestTypeRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.RequestTypes.AsQueryable();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var requestTypes = await query.OrderBy(e => e.Name).Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Domain.Entities.RequestType>
            {
                Data = requestTypes,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }

        public async Task<Domain.Entities.RequestType> GetById(int id)
        {
            var requestType = await _context.RequestTypes.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Request type not found!");

            return requestType;
        }

        public async Task<Domain.Entities.RequestType> Create(CreateRequestTypeRequest request)
        {
            var requestType = new Domain.Entities.RequestType
            {
                Name = request.Name,
                NameE = request.NameE
            };

            _context.RequestTypes.Add(requestType);

            await _context.SaveChangesAsync();

            return requestType;
        }

        public async Task<Domain.Entities.RequestType> Update(int id, CreateRequestTypeRequest request)
        {
            var requestType = await _context.RequestTypes.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Request type not found!");

            requestType.Name = request.Name;
            requestType.NameE = request.NameE;

            _context.RequestTypes.Update(requestType);

            await _context.SaveChangesAsync();

            return requestType;
        }

        public async Task<Domain.Entities.RequestType> Delete(int id)
        {
            var requestType = await _context.RequestTypes.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Request type not found!");

            _context.RequestTypes.Remove(requestType);

            await _context.SaveChangesAsync();

            return requestType;
        }
    }
}
