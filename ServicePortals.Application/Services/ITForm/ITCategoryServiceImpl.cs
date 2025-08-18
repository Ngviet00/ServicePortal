using ServicePortals.Application.Dtos.ITForm.Requests.ITCategory;
using ServicePortals.Application.Interfaces.ITForm;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITCategoryServiceImpl : ITCategoryService
    {
        public Task<object> Create(CreateITCategoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> Delete(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ITCategory>> GetAll(GetAllITCategoryRequest request)
        {
            throw new NotImplementedException();
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
