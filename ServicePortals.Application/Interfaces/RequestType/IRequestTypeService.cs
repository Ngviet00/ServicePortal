using ServicePortals.Application.Dtos.RequestType.Request;

namespace ServicePortals.Application.Interfaces.RequestType
{
    public interface IRequestTypeService
    {
        Task<PagedResults<Domain.Entities.RequestType>> GetAll(SearchRequestTypeRequest request);
        Task<Domain.Entities.RequestType> GetById(int id);
        Task<Domain.Entities.RequestType> Create(CreateRequestTypeRequest request);
        Task<Domain.Entities.RequestType> Update(int id, CreateRequestTypeRequest request);
        Task<Domain.Entities.RequestType> Delete(int id);
    }
}
