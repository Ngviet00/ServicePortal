using System.Data;
using System.Security.Claims;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Interfaces.Department;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.MemoNotification
{
    public class MemoNotificationService : IMemoNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDepartmentService _departmentService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;

        public MemoNotificationService(
            ApplicationDbContext context, 
            IViclockDapperContext viclockDapperContext,
            IHttpContextAccessor httpContextAccessor,
            IDepartmentService departmentService,
            IEmailService emailService,
            IUserService userService
        )
        {
            _context = context;
            _viclockDapperContext = viclockDapperContext;
            _httpContextAccessor = httpContextAccessor;
            _departmentService = departmentService;
            _emailService = emailService;
            _userService = userService;
        }

        public async Task<PagedResults<MemoNotificationDto>> GetAll(GetAllMemoNotiRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;

            string? CurrentUserCode = request.CurrentUserCode;

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                WITH NotificationData AS (
                    SELECT 
                        MN.Id,
                        MN.Title,
                        MN.Content,
                        MN.FromDate,
                        MN.ToDate,
                        MN.UserCodeCreated,
                        MN.CreatedAt,
		                MN.CreatedBy,
		                MN.Priority,
		                MN.Status,
                        MN.ApplyAllDepartment,
		                CASE 
                            WHEN MN.ApplyAllDepartment = 1 THEN NULL
                            ELSE (
                                SELECT STRING_AGG(BPTen, ', ')
                                FROM (
                                    SELECT DISTINCT BP.BPTen
                                    FROM memo_notification_departments MND2
                                    LEFT JOIN vs_new.dbo.tblBoPhan BP ON MND2.DepartmentId = BP.BPMa
                                    WHERE MND2.MemoNotificationId = MN.Id
                                ) AS UniqueDepartments
                            )
                        END AS DepartmentNames,
		                AF.Id AS ApplicationFormId,
		                AF.RequesterUserCode,
		                AF.RequestStatusId,
		                AF.CurrentOrgUnitId,
		                AF.CreatedAt AS ApplicationFormCreatedAt,
		                HAF.Id AS LatestHistoryApplicationFormId,
		                HAF.UserApproval,
		                HAF.UserCodeApproval,
		                HAF.ActionType,
		                HAF.Comment,
		                HAF.CreatedAt AS HistoryApplicationFormCreatedAt
                    FROM 
                        {Global.DbWeb}.dbo.memo_notifications AS MN
	                LEFT JOIN 
                        {Global.DbWeb}.dbo.application_forms AS AF
                        ON MN.ApplicationFormId = AF.Id
                    LEFT JOIN (
                        SELECT 
                            MND.MemoNotificationId,
                            STRING_AGG(BP.BPTen, ', ') AS DepartmentNames
                        FROM 
                            {Global.DbWeb}.dbo.memo_notification_departments AS MND
                        LEFT JOIN 
                            {Global.DbViClock}.dbo.tblBoPhan AS BP
                            ON MND.DepartmentId = BP.BPMa
                        GROUP BY 
                            MND.MemoNotificationId
                    ) AS NA
                        ON MN.Id = NA.MemoNotificationId
                    OUTER APPLY (
                        SELECT TOP 1 HAF.*
                        FROM {Global.DbWeb}.dbo.history_application_forms HAF
                        WHERE HAF.ApplicationFormId = AF.Id
                        ORDER BY HAF.CreatedAt DESC
                    ) AS HAF
                    WHERE 
                        MN.UserCodeCreated = @CurrentUserCode
                )
                    SELECT *
                    FROM NotificationData
                    ORDER BY CreatedAt DESC
                    OFFSET (@page - 1) * @pageSize ROWS
                    FETCH NEXT @pageSize ROWS ONLY;
            ";

            var parameters = new
            {
                CurrentUserCode,
                page,
                pageSize
            };

            var result = await connection.QueryAsync<MemoNotificationDto>(sql, parameters );

            var totalItems = await _context.MemoNotifications.CountAsync(e => e.UserCodeCreated == CurrentUserCode);

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PagedResults<MemoNotificationDto>
            {
                Data = (List<MemoNotificationDto>)result,
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

            var allDepartment = await _departmentService.GetAll();
            var nameDepartmentApplies = allDepartment.Where(e => departmentIds.Contains(e.BPMa)).Select(e => e.BPTen).Distinct().ToList();

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

            result.DepartmentNames = string.Join(", ", nameDepartmentApplies);

            return result;
        }

        public async Task<MemoNotificationDto> Create(CreateMemoNotiRequest request, IFormFile[] files)
        {
            int? orgUnitId = request.OrgUnitId;

            int requestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION;

            var userClaims = _httpContextAccessor.HttpContext?.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var isUnion = roleClaims?.Contains("UNION") ?? false;

            int? nextOrgUnitId = -1;
            int statusApplicationForm = -1;

            //công đoàn
            if (isUnion)
            {
                var workFlowStep = await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.OrgUnitContext == "ROLE_UNION");
                nextOrgUnitId = workFlowStep?.ToSpecificOrgUnitId;
                statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            }
            else
            {
                if (orgUnitId == null)
                {
                    throw new ValidationException("Thông tin vị trí chưa được cập nhật đủ, liên hệ bộ phận HR");
                }

                //case Mr.Ter tạo thông báo
                var workFlowStep = await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.FromOrgUnitId == orgUnitId);

                if (workFlowStep != null)
                {
                    statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    nextOrgUnitId = workFlowStep?.ToSpecificOrgUnitId ?? orgUnitId;
                }
                else
                {
                    var orgUnit = await _context.OrgUnits.FirstOrDefaultAsync(e => e.Id == orgUnitId);
                    var dept = orgUnit?.DeptId;

                    var workFlowStepOfStaff = await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.DepartmentId == dept && e.RequestTypeId == requestTypeId);

                    if (workFlowStepOfStaff != null)
                    {
                        nextOrgUnitId = workFlowStepOfStaff?.ToSpecificOrgUnitId;
                        statusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
                    }
                    else
                    {
                        throw new NotFoundException("Không tìm thấy luồng tạo thông báo của bạn, liên hệ Team IT");
                    }
                }
            }

            //add new application form
            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                RequesterUserCode = request.UserCodeCreated,
                RequestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION,
                RequestStatusId = statusApplicationForm,
                CreatedAt = DateTimeOffset.Now,
                CurrentOrgUnitId = nextOrgUnitId
            };

            var memoNotify = new Domain.Entities.MemoNotification
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                Title = request.Title,
                Content = request.Content,
                Status = request.Status,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                UserCodeCreated = request.UserCodeCreated,
                CreatedAt = request.CreatedAt,
                CreatedBy = request.CreatedBy,
                ApplyAllDepartment = request.ApplyAllDepartment,
            };

            _context.ApplicationForms.Add(applicationForm);
            _context.MemoNotifications.Add(memoNotify);

            //memo department
            if (request.ApplyAllDepartment == false)
            {
                List<MemoNotificationDepartment> memoNotificationDepartments = [];

                if (request.DepartmentIdApply != null)
                {
                    foreach (var item in request.DepartmentIdApply)
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
                        CreatedAt = DateTimeOffset.Now
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

            var receiveUser = await _userService.GetMultipleUserViclockByOrgUnitId(nextOrgUnitId ?? 0);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.EmailSendMemoNotificationNeedApproval(
                    receiveUser.Select(e => e.Email ?? "").ToList(),
                    null,
                    "New approval request notification",
                    TemplateEmail.EmailSendMemoNotificationNeedApproval(request.UrlFrontend ?? ""),
                    null,
                    true
                )
            );

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
                        CreatedAt = DateTimeOffset.Now
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

            var memoNotifyDept = await _context.MemoNotificationDepartments.Where(e => e.MemoNotificationId == id).ToListAsync();

            var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Notification not found to delete!");

            _context.MemoNotificationDepartments.RemoveRange(memoNotifyDept);

            _context.MemoNotifications.Remove(memoNotify);
            _context.AttachFiles.RemoveRange(attachFiles);

            await _context.SaveChangesAsync();

            return MemoNotifyMapper.ToDto(memoNotify);
        }

        public async Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId)
        {
            var userCode = _httpContextAccessor.HttpContext?.User?.FindFirst("user_code")?.Value;

            var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Date;

            var result = await _context.MemoNotifications
                .Include(e => e.ApplicationForm)
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
                    x.MemoNotification.ApplicationForm != null && 
                    x.MemoNotification.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.COMPLETE &&
                    x.MemoNotification.FromDate.HasValue && x.MemoNotification.FromDate.Value.Date <= today &&
                    x.MemoNotification.ToDate.HasValue && x.MemoNotification.ToDate.Value.Date >= today &&
                    (
                        x.MemoNotification.ApplyAllDepartment == true || 
                        x.MemoNotificationDepartment != null && 
                        (x.MemoNotificationDepartment.DepartmentId == DepartmentId || x.MemoNotification.UserCodeCreated == userCode)
                    )
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

        public async Task<PagedResults<MemoNotificationDto>> GetWaitApproval(MemoNotifyWaitApprovalRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;

            int? orgUnitId = request.OrgUnitId;

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                WITH NotificationData AS (
                    SELECT 
                        MN.Id,
                        MN.Title,
                        MN.Content,
                        MN.FromDate,
                        MN.ToDate,
                        MN.UserCodeCreated,
                        MN.CreatedAt,
                        MN.CreatedBy,
                        MN.Priority,
                        MN.Status,
                        MN.ApplyAllDepartment,
		                CASE 
                            WHEN MN.ApplyAllDepartment = 1 THEN NULL
                            ELSE (
                                SELECT STRING_AGG(BPTen, ', ')
                                FROM (
                                    SELECT DISTINCT BP.BPTen
                                    FROM memo_notification_departments MND2
                                    LEFT JOIN vs_new.dbo.tblBoPhan BP ON MND2.DepartmentId = BP.BPMa
                                    WHERE MND2.MemoNotificationId = MN.Id
                                ) AS UniqueDepartments
                            )
                        END AS DepartmentNames,
                        AF.Id AS ApplicationFormId,
                        AF.RequesterUserCode,
                        AF.RequestStatusId,
                        AF.CurrentOrgUnitId,
                        AF.CreatedAt AS ApplicationFormCreatedAt,
		                HAF.Id AS LatestHistoryApplicationFormId,
		                HAF.UserApproval,
		                HAF.UserCodeApproval,
		                HAF.ActionType,
		                HAF.Comment,
		                HAF.CreatedAt AS HistoryApplicationFormCreatedAt,
                        COUNT(*) OVER () AS TotalRecords
                        FROM 
                            {Global.DbWeb}.dbo.memo_notifications AS MN
                        INNER JOIN 
                                {Global.DbWeb}.dbo.application_forms AS AF
                            ON MN.ApplicationFormId = AF.Id
                        LEFT JOIN (
                            SELECT 
                                MND.MemoNotificationId,
                                STRING_AGG(BP.BPTen, ', ') AS DepartmentNames
                            FROM 
                                {Global.DbWeb}.dbo.memo_notification_departments AS MND
                            LEFT JOIN 
                                {Global.DbViClock}.dbo.tblBoPhan AS BP
                                ON MND.DepartmentId = BP.BPMa
                            GROUP BY 
                                MND.MemoNotificationId
                        ) AS NA
                            ON MN.Id = NA.MemoNotificationId
                        OUTER APPLY (
                            SELECT TOP 1 HAF.*
                            FROM {Global.DbWeb}.dbo.history_application_forms HAF
                            WHERE HAF.ApplicationFormId = AF.Id
                            ORDER BY HAF.CreatedAt DESC
                        ) AS HAF
                        WHERE 
                            AF.CurrentOrgUnitId = @orgUnitId AND AF.RequestStatusId NOT IN (@Complete, @Reject)
                    )
                    SELECT * FROM NotificationData ORDER BY CreatedAt DESC OFFSET (@page - 1) * @pageSize ROWS FETCH NEXT @pageSize ROWS ONLY;
            ";

            var parameters = new
            {
                orgUnitId,
                page,
                pageSize,
                Complete = (int)StatusApplicationFormEnum.COMPLETE,
                Reject = (int)StatusApplicationFormEnum.REJECT,
            };

            var result = await connection.QueryAsync<MemoNotificationDto>(sql, parameters);

            int totalItems = 0;

            if (result.Any())
            {
                totalItems = result?.FirstOrDefault()?.TotalRecords ?? 0;
            }

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PagedResults<MemoNotificationDto>
            {
                Data = (List<MemoNotificationDto>)result,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResults<MemoNotificationDto>> GetHistoryApproval(HistoryWaitApprovalMemoNotifyRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;

            string? CurrentUserCode = request.CurrentUserCode;

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            string sql = $@"
                WITH NotificationData AS (
                    SELECT 
                        MN.Id,
                        MN.Title,
                        MN.Content,
                        MN.FromDate,
                        MN.ToDate,
                        MN.UserCodeCreated,
                        MN.CreatedAt,
		                MN.CreatedBy,
		                MN.Priority,
		                MN.Status,
                        MN.ApplyAllDepartment,
		                CASE 
                            WHEN MN.ApplyAllDepartment = 1 THEN NULL
                            ELSE (
                                SELECT STRING_AGG(BPTen, ', ')
                                FROM (
                                    SELECT DISTINCT BP.BPTen
                                    FROM memo_notification_departments MND2
                                    LEFT JOIN vs_new.dbo.tblBoPhan BP ON MND2.DepartmentId = BP.BPMa
                                    WHERE MND2.MemoNotificationId = MN.Id
                                ) AS UniqueDepartments
                            )
                        END AS DepartmentNames,
		                AF.Id AS ApplicationFormId,
		                AF.RequesterUserCode,
		                AF.RequestStatusId,
		                AF.CurrentOrgUnitId,
		                AF.CreatedAt AS ApplicationFormCreatedAt,
		                HAF.Id AS LatestHistoryApplicationFormId,
		                HAF.UserApproval,
		                HAF.UserCodeApproval,
		                HAF.ActionType,
		                HAF.Comment,
		                HAF.CreatedAt AS HistoryApplicationFormCreatedAt,
		                COUNT(*) OVER () AS TotalRecords
                    FROM 
                        {Global.DbWeb}.dbo.memo_notifications AS MN
	                LEFT JOIN 
                        {Global.DbWeb}.dbo.application_forms AS AF
                        ON MN.ApplicationFormId = AF.Id
	                INNER JOIN 
	                    {Global.DbWeb}.dbo.history_application_forms AS HAF
                        ON HAF.ApplicationFormId = AF.Id
                    LEFT JOIN (
                        SELECT 
                            MND.MemoNotificationId,
                            STRING_AGG(BP.BPTen, ', ') AS DepartmentNames
                        FROM 
                            {Global.DbWeb}.dbo.memo_notification_departments AS MND
                        LEFT JOIN 
                            {Global.DbViClock}.dbo.tblBoPhan AS BP
                            ON MND.DepartmentId = BP.BPMa
                        GROUP BY 
                            MND.MemoNotificationId
                    ) AS NA
                        ON MN.Id = NA.MemoNotificationId
                    WHERE 
                        HAF.UserCodeApproval = @UserCode
                )
                SELECT *
                FROM NotificationData
                ORDER BY CreatedAt DESC
                OFFSET (@page - 1) * @pageSize ROWS
                FETCH NEXT @pageSize ROWS ONLY;
            ";

            var parameters = new
            {
                UserCode = CurrentUserCode,
                page,
                pageSize
            };

            var result = await connection.QueryAsync<MemoNotificationDto>(sql, parameters);
            int totalItems = 0;

            if (result.Any())
            {
                totalItems = result?.FirstOrDefault()?.TotalRecords ?? 0;
            }

            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return new PagedResults<MemoNotificationDto>
            {
                Data = (List<MemoNotificationDto>)result,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<object> Approval(ApprovalMemoNotifyRequest request)
        {
            var orgUnitId = request.OrgUnitId;

            var memoNotify = await _context.MemoNotifications.Include(e => e.ApplicationForm).FirstOrDefaultAsync(e => e.Id == request.MemoNotificationId);

            if (memoNotify == null)
            {
                throw new NotFoundException("Memo notify not found");
            }

            var applicationForm = memoNotify.ApplicationForm;

            if (applicationForm == null)
            {
                throw new NotFoundException("Application form memo notify not found");
            }

            applicationForm.Id = applicationForm.Id;

            var historyApplicationForm = new HistoryApplicationForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm?.Id,
                UserApproval = request.UserNameApproval,
                UserCodeApproval = request.UserCodeApproval,
                Comment = request.Note,
                ActionType = request.Status == true ? "APPROVAL" : "REJECT",
                CreatedAt = DateTimeOffset.Now
            };

            int? nextOrgUnitId = -1;
            bool isFinal = false;

            //reject
            if (request.Status == false)
            {
                applicationForm!.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
            }
            else //approval
            {
                if (applicationForm!.RequestStatusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL) //if is final -> approval will publish
                {
                    applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
                    isFinal = true;
                }
                else
                {
                    var workFlowStep = await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION && e.FromOrgUnitId == orgUnitId);

                    if (workFlowStep != null)
                    {
                        applicationForm.RequestStatusId = workFlowStep.IsFinal == true ? (int)StatusApplicationFormEnum.FINAL_APPROVAL : (int)StatusApplicationFormEnum.IN_PROCESS;
                        nextOrgUnitId = workFlowStep?.ToSpecificOrgUnitId ?? orgUnitId;
                    }
                    else
                    {
                        throw new NotFoundException("Không tìm thấy luồng tạo thông báo của bạn, liên hệ Team IT");
                    }
                }
            }

            applicationForm.CurrentOrgUnitId = nextOrgUnitId;

            _context.HistoryApplicationForms.Add(historyApplicationForm);

            _context.ApplicationForms.Update(applicationForm);

            await _context.SaveChangesAsync();

            if (request.Status == false || request.Status == true && isFinal) //gửi email cho người tạo thông báo là complete or reject
            {
                var userRequest = await _context.Users.Where(e => e.UserCode == memoNotify.UserCodeCreated).ToListAsync();
                bool isApproved = request.Status == true;
                string title = isApproved ? "approved" : "reject";
                
                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.EmailSendMemoNotificationHasBeenCompletedOrReject(
                        userRequest.Select(e => e.Email ?? "").ToList(),
                        null,
                        $"Your request create notification has been {title}",
                        TemplateEmail.EmailSendMemoNotificationHasBeenCompletedOrReject(request.UrlFrontend ?? "", isApproved),
                        null,
                        true
                    )
                );
            }
            else //gửi cho người tiếp theo duyệt
            {
                var receiveUser = await _userService.GetMultipleUserViclockByOrgUnitId(nextOrgUnitId ?? 0);

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.EmailSendMemoNotificationNeedApproval(
                        receiveUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "New approval request notification",
                        TemplateEmail.EmailSendMemoNotificationNeedApproval(request.UrlFrontend ?? ""),
                        null,
                        true
                    )
                );
            }

            return true;
        }

        public async Task<int> CountWaitApprovalMemoNotification(int orgUnitId)
        {
            var result = await _context.MemoNotifications
                .Include(e => e.ApplicationForm)
                .Where(e => 
                    e.ApplicationForm != null &&
                    e.ApplicationForm.CurrentOrgUnitId == orgUnitId && 
                    (
                        e.ApplicationForm.RequestTypeId != (int)StatusApplicationFormEnum.COMPLETE || 
                        e.ApplicationForm.RequestStatusId != (int)StatusApplicationFormEnum.REJECT
                    )
                )
                .CountAsync();

            return result;
        }
    }
}
