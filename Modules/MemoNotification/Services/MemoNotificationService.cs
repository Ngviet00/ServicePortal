using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.MemoNotification.DTO.Requests;
using ServicePortal.Modules.MemoNotification.Services.Interfaces;

namespace ServicePortal.Modules.MemoNotification.Services
{
    public class MemoNotificationService : IMemoNotificationService
    {
        private readonly ApplicationDbContext _context;

        public MemoNotificationService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<Domain.Entities.MemoNotification>> GetAll(GetAllMemoNotiDto request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.MemoNotifications.AsQueryable();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var memoNotifications = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var result = new PagedResults<Domain.Entities.MemoNotification>
            {
                Data = memoNotifications,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return result;
        }
        public async Task<Domain.Entities.MemoNotification> GetById(Guid id)
        {
            var item = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Notification not found to delete!");

            return item;
        }

        public async Task<Domain.Entities.MemoNotification> Create(CreateMemoNotiDto dto)
        {
            var memoNotify = new Domain.Entities.MemoNotification
            {
                Title = dto.Title,
                Content = dto.Content,
                Status = dto.Status,
                CreatedByDepartmentId = dto.CreatedByDepartmentId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                UserCodeCreated = dto.UserCodeCreated,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                ApplyAllDepartment = dto.ApplyAllDepartment,
            };

            _context.MemoNotifications.Add(memoNotify);

            //if (dto.ApplyAllDepartment == false)
            //{
            //    List<MemoNotificationDepartment> memoNotificationDepartments = [];
                
            //    if (dto.DepartmentIdApply != null)
            //    {
            //        foreach (var item in dto.DepartmentIdApply)
            //        {
            //            memoNotificationDepartments.Add(new MemoNotificationDepartment
            //            {
            //                MemoNotificationId = memoNotify.Id,
            //                DepartmentId = int.Parse(item)
            //            });
            //        }
            //        _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
            //    }
            //}

            await _context.SaveChangesAsync();

            return memoNotify;
        }

        public async Task<Domain.Entities.MemoNotification> Update(Guid id, CreateMemoNotiDto dto)
        {
            var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo notification not found!");

            memoNotify.Title = dto.Title;
            memoNotify.Content = dto.Content;
            memoNotify.Status = dto.Status;
            memoNotify.FromDate = dto.FromDate;
            memoNotify.ToDate = dto.ToDate;
            memoNotify.ApplyAllDepartment = dto.ApplyAllDepartment;
            memoNotify.UpdatedBy = dto.UpdatedBy;
            memoNotify.UpdatedAt = dto.UpdatedAt;

            _context.MemoNotifications.Update(memoNotify);

            //var memoNotifyDept = await _context.MemoNotificationDepartments.Where(e => e.MemoNotificationId == memoNotify.Id).ToListAsync();

            //_context.MemoNotificationDepartments.RemoveRange(memoNotifyDept);

            //if (dto.ApplyAllDepartment == false)
            //{
            //    List<MemoNotificationDepartment> memoNotificationDepartments = [];

            //    if (dto.DepartmentIdApply != null)
            //    {
            //        foreach (var item in dto.DepartmentIdApply)
            //        {
            //            memoNotificationDepartments.Add(new MemoNotificationDepartment
            //            {
            //                MemoNotificationId = memoNotify.Id,
            //                DepartmentId = int.Parse(item)
            //            });
            //        }
            //        _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
            //    }
            //}

            await _context.SaveChangesAsync();

            return memoNotify;
        }

        public async Task<Domain.Entities.MemoNotification> Delete(Guid id)
        {
            var item = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Notification not found to delete!");

            _context.MemoNotifications.Remove(item);

            await _context.SaveChangesAsync();

            return item;
        }

        public async Task<List<Domain.Entities.MemoNotification>> GetAllInHomePage()
        {
            var now = DateTime.UtcNow.AddHours(7);

            var item = await _context.MemoNotifications
                .Where(x => x.Status == true && x.FromDate <= now && x.ToDate >= now)
                .ToListAsync();

            return item;
        }
    }
}
