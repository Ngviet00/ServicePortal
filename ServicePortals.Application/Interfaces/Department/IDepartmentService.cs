using ServicePortals.Application.Dtos.Department.Responses;

namespace ServicePortals.Application.Interfaces.Department
{
    public interface IDepartmentService
    {
        Task<List<GetAllDepartmentResponse>> GetAll();
    }
}
