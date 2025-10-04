using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.ITForm.Requests.ITCategory;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITCategoryServiceImpl : ITCategoryService
    {
        private readonly ApplicationDbContext _context;
        public ITCategoryServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<object> Create(CreateITCategoryRequest request)
        {
            var newItem = new ITCategory
            {
                Name = request.Name,
                Code = request.Code
            };

            _context.ITCategories.Add(newItem);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Delete(int Id)
        {
            var item = await GetById(Id);

            if (item != null)
            {
                _context.ITCategories.Remove(item);
                await _context.SaveChangesAsync();
                
                return true;
            }

            return false;
        }

        public async Task<List<ITCategory>> GetAll()
        {
            return await _context.ITCategories.ToListAsync();
        }

        public async Task<ITCategory?> GetById(int Id)
        {
            return await _context.ITCategories.FirstOrDefaultAsync(e => e.Id == Id) ?? throw new NotFoundException("IT Category not found");
        }

        public async Task<object> Update(int Id, UpdateITCategoryRequest request)
        {
            var item = await GetById(Id);

            if (item != null)
            {
                item.Name = request.Name;
                item.Code = request.Code;

                _context.ITCategories.Update(item);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
