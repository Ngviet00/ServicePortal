using Microsoft.AspNetCore.Http;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;

namespace ServicePortals.Application.Interfaces.MemoNotification
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<Domain.Entities.MemoNotification>> GetAll(GetAllMemoNotificationRequest request);
        Task<Domain.Entities.MemoNotification?> GetById(Guid id);
        Task<object> Create(CreateMemoNotificationRequest request, IFormFile[] files);
        Task<object> Update(Guid id, CreateMemoNotificationRequest request, IFormFile[] files);
        Task<object> Delete(Guid id);
        Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId);
        Task<Domain.Entities.File> GetFileDownload(Guid id);
        Task<object> Approval(ApprovalRequest request);
    }
}
