using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.InternalMemoHR.Requests;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.InternalMemoHR
{
    public interface InternalMemoHrService
    {
        Task<PagedResults<ApplicationForm>> GetList(GetListInternalMemoHrRequest request);
        Task<ApplicationForm> GetDetailInternalMemoByApplicationFormCode(string applicationFormCode);
        Task<object> Create(CreateInternalMemoHrRequest request);
        Task<object> Update(string applicationFormCode, CreateInternalMemoHrRequest request);
        Task<object> Delete(string applicationFormCode);
        Task<object> Approval(ApprovalRequest request);
    }
}
