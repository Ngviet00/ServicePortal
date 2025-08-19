using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.ITForm.Requests.ITCategory;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITCategoryServiceImpl : ITCategoryService
    {
        private readonly ApplicationDbContext _context;
        public ITCategoryServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<object> Create(CreateITCategoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ITCategory>> GetAll(GetAllITCategoryRequest request)
        {
            return await _context.ITCategories.ToListAsync();
        }

        public Task<ITCategory> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<object> Update(int Id, UpdateITCategoryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
