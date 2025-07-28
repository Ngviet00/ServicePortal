using ServicePortals.Shared.SharedDto;

namespace ServicePortals.Application.Interfaces.Department
{
    public interface IDepartmentService
    {
        Task<List<GetAllDepartmentResponse>> GetAll();
        Task<List<string>> GetAllWithDistinctName();
    }
}
