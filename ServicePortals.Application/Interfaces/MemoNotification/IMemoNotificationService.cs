using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;

namespace ServicePortals.Application.Interfaces.MemoNotification
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<MemoNotificationDto>> GetAll(GetAllMemoNotiRequest request);
        Task<PagedResults<MemoNotificationDto>> GetWaitApproval(MemoNotifyWaitApprovalRequest request);
        Task<PagedResults<MemoNotificationDto>> GetHistoryApproval(HistoryWaitApprovalMemoNotifyRequest request);
        Task<MemoNotificationDto> GetById(Guid id);
        Task<MemoNotificationDto> Create(CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Update(Guid id, CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Delete(Guid id);
        Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId);
        Task<Domain.Entities.File> GetFileDownload(Guid id);
        Task<object> Approval(ApprovalMemoNotifyRequest request);
        Task<int> CountWaitApprovalMemoNotification(int orgUnitId);
        Task<PendingApprovalList> WaitApproval(ListWaitApprovalRequest request, ClaimsPrincipal userClaims);
    }
}
