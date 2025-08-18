using Microsoft.AspNetCore.Http;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;

namespace ServicePortals.Application.Interfaces.MemoNotification
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<Domain.Entities.MemoNotification>> GetAll(GetAllMemoNotiRequest request);
        Task<Domain.Entities.MemoNotification?> GetById(Guid id);
        Task<object> Create(CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Update(Guid id, CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Delete(Guid id);
        Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId);
        Task<Domain.Entities.File> GetFileDownload(Guid id);
        Task<object> Approval(ApprovalRequest request);
    }
}
