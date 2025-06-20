using ServicePortal.Applications.Modules.Department.DTO.Responses;

namespace ServicePortal.Applications.Modules.Department.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<GetAllDepartmentResponse>> GetAll();
    }
}
