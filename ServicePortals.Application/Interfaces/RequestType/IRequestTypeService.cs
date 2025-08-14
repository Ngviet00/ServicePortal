using ServicePortals.Application.Dtos.RequestType.Request;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.RequestType
{
    public interface IRequestTypeService
    {
        Task<PagedResults<Entities.RequestType>> GetAll(SearchRequestTypeRequest request);
        Task<Entities.RequestType> GetById(int id);
        Task<Entities.RequestType> Create(CreateRequestTypeRequest request);
        Task<Entities.RequestType> Update(int id, CreateRequestTypeRequest request);
        Task<Entities.RequestType> Delete(int id);
    }
}
