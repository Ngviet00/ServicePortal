using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Infrastructure.Services.MemoNotification
{
    public class MemoNotificationService : IMemoNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;

        public MemoNotificationService(
            ApplicationDbContext context, 
            IViclockDapperContext viclockDapperContext
        )
        {
            _context = context;
            _viclockDapperContext = viclockDapperContext;
        }

        public async Task<PagedResults<MemoNotificationDto>> GetAll(GetAllMemoNotiRequest request)
        {
            double pageSize = request.PageSize;
            double page = request.Page;

            int skip = (int)((page - 1) * pageSize);
            int take = (int)pageSize;

            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Name == request.RoleName);

            int? roleId = 0;

            if (role == null)
            {
                roleId = Global.DefaultDepartmentIdHR;
            }
            else
            {
                roleId = role.Id;
            }

            string sql = $@"
                WITH DistinctDeptNames AS (
                    SELECT 
                        mn.Id,
                        d.BPTen
                    FROM {Global.DbWeb}.dbo.memo_notifications AS mn
                    LEFT JOIN {Global.DbWeb}.dbo.memo_notification_departments AS mnd
                        ON mn.Id = mnd.MemoNotificationId
                    LEFT JOIN {Global.DbViClock}.dbo.tblBoPhan AS d
                        ON mnd.DepartmentId = d.BPMa
                    WHERE mn.CreatedByRoleId = @roleId
                    GROUP BY mn.Id, d.BPTen
                ),
                Base AS (
                    SELECT
                        mn.Id,
                        mn.Title,
                        mn.Content,
                        mn.FromDate,
                        mn.ToDate,
                        mn.UserCodeCreated,
                        mn.CreatedBy,
                        mn.CreatedAt,
                        mn.UpdatedBy,
                        mn.UpdatedAt,
                        mn.CreatedByRoleId,
                        mn.Priority,
                        mn.Status,
                        mn.ApplyAllDepartment,
                        STRING_AGG(d.BPTen, ', ') AS DepartmentNames
                    FROM {Global.DbWeb}.dbo.memo_notifications AS mn
                    LEFT JOIN DistinctDeptNames AS d
                        ON mn.Id = d.Id
                    WHERE mn.CreatedByRoleId = @roleId
                    GROUP BY 
                        mn.Id, mn.Title, mn.Content, mn.FromDate, mn.ToDate,
                        mn.UserCodeCreated, mn.CreatedBy, mn.CreatedAt,
                        mn.UpdatedBy, mn.UpdatedAt, mn.CreatedByRoleId,
                        mn.Priority, mn.Status, mn.ApplyAllDepartment
                )
             SELECT * FROM Base ORDER BY CreatedAt DESC OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;";

            string countSql = $@"
                SELECT COUNT(DISTINCT mn.Id)
                FROM {Global.DbWeb}.dbo.memo_notifications AS mn
                WHERE mn.CreatedByRoleId = @roleId;
            ";

            var parameters = new
            {
                roleId,
                skip,
                take
            };

            var data = await _viclockDapperContext.QueryAsync<MemoNotificationDto>(sql, parameters);
            var totalItems = await _viclockDapperContext.QueryFirstOrDefaultAsync<int>(countSql, new { roleId });

            int totalPages = (int)Math.Ceiling(totalItems / pageSize);

            return new PagedResults<MemoNotificationDto>
            {
                Data = (List<MemoNotificationDto>)data,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<MemoNotificationDto> GetById(Guid id)
        {
            var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo Notification not found!");

            var departmentIds = await _context.MemoNotificationDepartments
                .Where(mnd => mnd.MemoNotificationId == id)
                .Select(mnd => mnd.DepartmentId)
                .ToArrayAsync();

            var files = await _context.AttachFiles
                .Where(e => e.EntityId == memoNotify.Id && e.EntityType == nameof(MemoNotification))
                .Join(_context.Files,
                    af => af.FileId,
                    f => f.Id,
                    (afr, af) => new Domain.Entities.File
                    {
                        Id = af.Id,
                        FileName = af.FileName,
                        ContentType = af.ContentType
                    }
                ).ToListAsync();

            var result = new MemoNotificationDto
            {
                Id = memoNotify.Id,
                Title = memoNotify.Title,
                Content = memoNotify.Content,
                Status = memoNotify.Status,
                ApplyAllDepartment = memoNotify.ApplyAllDepartment,
                FromDate = memoNotify.FromDate,
                ToDate = memoNotify.ToDate,
                UserCodeCreated = memoNotify.UserCodeCreated,
                CreatedAt = memoNotify.CreatedAt,
                CreatedBy = memoNotify.CreatedBy,
                Priority = memoNotify.Priority,
                DepartmentIdApply = [.. departmentIds],
                Files = files
            };

            return result;
        }

        public async Task<MemoNotificationDto> Create(CreateMemoNotiRequest dto, IFormFile[] files)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(e => e.Name == dto.RoleNameCreated);

            int? roleId = 0;

            if (role == null)
            {
                roleId = Global.DefaultDepartmentIdHR;
            }
            else
            {
                roleId = role.Id;
            }

            var memoNotify = new Domain.Entities.MemoNotification
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Content = dto.Content,
                Status = dto.Status,
                CreatedByRoleId = roleId,
                FromDate = dto.FromDate,
                ToDate = dto.ToDate,
                UserCodeCreated = dto.UserCodeCreated,
                CreatedAt = dto.CreatedAt,
                CreatedBy = dto.CreatedBy,
                ApplyAllDepartment = dto.ApplyAllDepartment,
            };

            _context.MemoNotifications.Add(memoNotify);

            //memo department
            if (dto.ApplyAllDepartment == false)
            {
                List<MemoNotificationDepartment> memoNotificationDepartments = [];

                if (dto.DepartmentIdApply != null)
                {
                    foreach (var item in dto.DepartmentIdApply)
                    {
                        memoNotificationDepartments.Add(new MemoNotificationDepartment
                        {
                            MemoNotificationId = memoNotify.Id,
                            DepartmentId = item
                        });
                    }
                    _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
                }
            }

            //memo file
            if (files.Length > 0)
            {
                List<Domain.Entities.File> listEntityFiles = [];
                List<AttachFile> listAttachFiles = [];

                foreach (var file in files)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    var attach = new Domain.Entities.File
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FileData = ms.ToArray(),
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    listEntityFiles.Add(attach);

                    var attachFile = new AttachFile
                    {
                        FileId = attach.Id,
                        EntityType = nameof(MemoNotification),
                        EntityId = memoNotify.Id
                    };

                    listAttachFiles.Add(attachFile);
                }

                _context.Files.AddRange(listEntityFiles);
                _context.AttachFiles.AddRange(listAttachFiles);
            }

            await _context.SaveChangesAsync();

            return MemoNotifyMapper.ToDto(memoNotify);
        }

        public async Task<MemoNotificationDto> Update(Guid id, CreateMemoNotiRequest dto, IFormFile[] files)
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

            //delete file
            if (dto.DeleteFiles != null && dto.DeleteFiles.Count() > 0)
            {
                var fileDelete = await _context.Files.Where(at => dto.DeleteFiles.Contains(at.Id.ToString() ?? "")).ToListAsync();

                _context.Files.RemoveRange(fileDelete);
            }

            //memo file
            if (files.Length > 0)
            {
                List<Domain.Entities.File> listEntityFiles = [];
                List<AttachFile> listAttachFiles = [];

                foreach (var file in files)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    var attach = new Domain.Entities.File
                    {
                        Id = Guid.NewGuid(),
                        FileName = file.FileName,
                        ContentType = file.ContentType,
                        FileData = ms.ToArray(),
                        CreatedAt = DateTimeOffset.UtcNow
                    };
                    listEntityFiles.Add(attach);

                    var attachfile = new AttachFile
                    {
                        FileId = attach.Id,
                        EntityId = memoNotify.Id,
                        EntityType = nameof(MemoNotification)
                    };

                    listAttachFiles.Add(attachfile);
                }

                _context.Files.AddRange(listEntityFiles);
                _context.AttachFiles.AddRange(listAttachFiles);
            }

            var memoNotifyDept = await _context.MemoNotificationDepartments.Where(e => e.MemoNotificationId == memoNotify.Id).ToListAsync();

            _context.MemoNotificationDepartments.RemoveRange(memoNotifyDept);

            if (dto.ApplyAllDepartment == false)
            {
                List<MemoNotificationDepartment> memoNotificationDepartments = [];

                if (dto.DepartmentIdApply != null)
                {
                    foreach (var item in dto.DepartmentIdApply)
                    {
                        memoNotificationDepartments.Add(new MemoNotificationDepartment
                        {
                            MemoNotificationId = memoNotify.Id,
                            DepartmentId = item
                        });
                    }
                    _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
                }
            }

            await _context.SaveChangesAsync();

            return MemoNotifyMapper.ToDto(memoNotify);
        }

        public async Task<MemoNotificationDto> Delete(Guid id)
        {
            var attachFiles = await _context.AttachFiles.Where(e => e.EntityType == nameof(MemoNotification) && e.EntityId == id).ToListAsync();

            var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Notification not found to delete!");

            _context.MemoNotifications.Remove(memoNotify);
            _context.AttachFiles.RemoveRange(attachFiles);

            await _context.SaveChangesAsync();

            return MemoNotifyMapper.ToDto(memoNotify);
        }

        public async Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId)
        {
            var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Date;

            var result = await _context.MemoNotifications
                .GroupJoin(
                    _context.MemoNotificationDepartments,
                    memo => memo.Id,
                    memoDept => memoDept.MemoNotificationId,
                    (memo, memoDeptGroup) => new { memo, memoDeptGroup }
                )
                .SelectMany(
                    x => x.memoDeptGroup.DefaultIfEmpty(),
                    (x, memoDept) => new { MemoNotification = x.memo, MemoNotificationDepartment = memoDept }
                )
                .Where(x =>
                    x.MemoNotification.Status == true &&
                    x.MemoNotification.FromDate.HasValue && x.MemoNotification.FromDate.Value.Date <= today &&
                    x.MemoNotification.ToDate.HasValue && x.MemoNotification.ToDate.Value.Date >= today &&
                    (x.MemoNotification.ApplyAllDepartment == true || x.MemoNotificationDepartment != null && x.MemoNotificationDepartment.DepartmentId == DepartmentId)
                )
                .Select(x => x.MemoNotification)
                .Distinct()
                .ToListAsync();

            return MemoNotifyMapper.ToDtoList(result);
        }

        public async Task<Domain.Entities.File> GetFileDownload(Guid id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo Notification not found!");

            return file;
        }
    }
}
