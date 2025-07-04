﻿using Microsoft.AspNetCore.Http;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;

namespace ServicePortals.Application.Interfaces.MemoNotification
{
    public interface IMemoNotificationService
    {
        Task<PagedResults<MemoNotificationDto>> GetAll(GetAllMemoNotiRequest request);
        Task<MemoNotificationDto> GetById(Guid id);
        Task<MemoNotificationDto> Create(CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Update(Guid id, CreateMemoNotiRequest dto, IFormFile[] files);
        Task<MemoNotificationDto> Delete(Guid id);
        Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId);
        Task<Domain.Entities.File> GetFileDownload(Guid id);
    }
}
