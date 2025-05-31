using ServicePortal.Common;
using ServicePortal.Modules.MemoNotification.DTO.Requests;

namespace ServicePortal.Modules.MemoNotification.Services.Interfaces
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<Domain.Entities.MemoNotification>> GetAll(GetAllMemoNotiDto request);
        Task<Domain.Entities.MemoNotification> GetById(Guid id);
        Task<Domain.Entities.MemoNotification> Create(CreateMemoNotiDto dto);
        Task<Domain.Entities.MemoNotification> Update(Guid id, CreateMemoNotiDto dto);
        Task<Domain.Entities.MemoNotification> Delete(Guid id);
        Task<List<Domain.Entities.MemoNotification>> GetAllInHomePage();
    }
}
