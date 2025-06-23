using ServicePortals.Application.Dtos.Position.Responses;

namespace ServicePortals.Application.Interfaces.Position
{
    public interface IPositionService
    {
        Task<List<GetAllPositionResponse>> GetAll();
    }
}
