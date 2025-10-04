using ServicePortals.Application.Dtos.ITForm.Requests.ITCategory;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.ITForm
{
    public interface ITCategoryService
    {
        Task<List<ITCategory>> GetAll();
        Task<ITCategory?> GetById(int Id);
        Task<object> Create(CreateITCategoryRequest request);
        Task<object> Update(int Id, UpdateITCategoryRequest request);
        Task<object> Delete(int Id);
    }
}
