using ServicePortal.Applications.Modules.Position.DTO.Responses;

namespace ServicePortal.Applications.Modules.Position.Services.Interfaces
{
    public interface IPositionService
    {
        Task<List<GetAllPositionResponse>> GetAll();
    }
}
