using Microsoft.EntityFrameworkCore;
using ServicePortal.Infrastructure.Cache;
using ServicePortals.Application.Dtos.TypeLeave;
using ServicePortals.Application.Dtos.TypeLeave.Requests;
using ServicePortals.Application.Interfaces.TypeLeave;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.TypeLeave
{
    /// <summary>
    /// 
    /// CRUD loại phép
    /// 
    /// </summary>
    public class TypeLeaveService : ITypeLeaveService
    {
        private readonly ApplicationDbContext _context;

        private readonly ICacheService _cacheService;

        private const string CacheKeyGetAllTypeLeave = "get_all_type_leave";

        public TypeLeaveService(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        public async Task<List<Domain.Entities.TypeLeave>> GetAll(GetAllTypeLeaveRequest request)
        {
            var items = await _cacheService.GetOrCreateAsync(CacheKeyGetAllTypeLeave, async () =>
            {
                return await _context.TypeLeaves.ToListAsync();
            }, expireMinutes: 1440);

            return items;
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
                NameV = dto.NameV,
                Code = dto.Code,
            };

            _context.TypeLeaves.Add(typeLeave);

            await _context.SaveChangesAsync();

            _cacheService.Remove(CacheKeyGetAllTypeLeave);

            return typeLeave;
        }

        public async Task<Domain.Entities.TypeLeave> Update(int id, TypeLeaveDto dto)
        {
            var typeLeave = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Type leave not found!");

            typeLeave.Id = id;
            typeLeave.Name = dto.Name;
            typeLeave.NameV = dto.NameV;
            typeLeave.Code = dto.Code;

            _context.TypeLeaves.Update(typeLeave);

            await _context.SaveChangesAsync();

            _cacheService.Remove(CacheKeyGetAllTypeLeave);

            return typeLeave;
        }

        public async Task<Domain.Entities.TypeLeave> Delete(int id)
        {
            var typeLeave = await _context.TypeLeaves.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Type leave not found!");

            _context.TypeLeaves.Remove(typeLeave);

            await _context.SaveChangesAsync();

            _cacheService.Remove(CacheKeyGetAllTypeLeave);

            return typeLeave;
        }
    }
}
