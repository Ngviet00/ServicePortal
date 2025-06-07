using ServicePortal.Applications.Modules.MemoNotification.DTO;
using ServicePortal.Applications.Modules.MemoNotification.DTO.Requests;
using ServicePortal.Common;

namespace ServicePortal.Applications.Modules.MemoNotification.Services.Interfaces
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<MemoNotificationDto>> GetAll(GetAllMemoNotiDto request);
        Task<MemoNotificationDto> GetById(Guid id);
        Task<Domain.Entities.MemoNotification> Create(CreateMemoNotiDto dto, IFormFile[] files);
        Task<Domain.Entities.MemoNotification> Update(Guid id, CreateMemoNotiDto dto, IFormFile[] files);
        Task<Domain.Entities.MemoNotification> Delete(Guid id);
        Task<List<Domain.Entities.MemoNotification>> GetAllInHomePage(int? DepartmentId);
        Task<Domain.Entities.AttachFiles> GetFileDownload(Guid id);
    }
}
