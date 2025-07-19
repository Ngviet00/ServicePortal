using ServicePortals.Application.Dtos.DelegatedTemp;
using ServicePortals.Application.Dtos.DelegatedTemp.Requests;
using ServicePortals.Application.Dtos.DelegatedTemp.Responses;

namespace ServicePortals.Application.Interfaces.DelegatedTemp
{
    public interface IDelegatedTempService
    {
        Task<DelegatedTempDto> AddNew(CreateDelegatedTempRequest request);

        Task<List<GetAllDelegatedTempResponse>> GetAll(DelegatedTempDto request);

        Task<DelegatedTempDto> Delete(DelegatedTempDto request);
    }
}
