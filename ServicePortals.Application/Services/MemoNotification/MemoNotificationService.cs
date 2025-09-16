using System.Data;
using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Infrastructure.Mappers;
using ServicePortals.Shared.Exceptions;
using Entities = ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Services.MemoNotification
{
    public class MemoNotificationService : IMemoNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly int GM_Department = 6;

        public MemoNotificationService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IConfiguration configuration
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _configuration = configuration;
        }

        public async Task<PagedResults<Entities.MemoNotification>> GetAll(GetAllMemoNotiRequest request)
        {
            return null;
            //int pageSize = request.PageSize;
            //int page = request.Page;
            //string? userCode = request.CurrentUserCode;

            //var query = _context.MemoNotifications.AsQueryable();

            //query = query.Where(e => e.ApplicationForm != null && e.ApplicationForm.UserCodeCreated == userCode);

            //var totalItems = await query.CountAsync();
            //int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            //var projectedQuery = SelectMemoNotify(query)
            //    .Select(e => new Entities.MemoNotification
            //    {
            //        Id = e.Id,
            //        Title = e.Title,
            //        Content = e.Content,
            //        Priority = e.Priority,
            //        Status = e.Status,
            //        FromDate = e.FromDate,
            //        ToDate = e.ToDate,
            //        ApplyAllDepartment = e.ApplyAllDepartment,
            //        CreatedAt = e.CreatedAt,
            //        OrgUnit = e.OrgUnit,
            //        MemoNotificationDepartments = e.MemoNotificationDepartments,
            //        ApplicationForm = e.ApplicationForm
            //    });

            //var results = await projectedQuery
            //    .OrderByDescending(e => e.CreatedAt)
            //    .Skip(((page - 1) * pageSize))
            //    .Take(pageSize)
            //    .ToListAsync();

            //return new PagedResults<Entities.MemoNotification>
            //{
            //    Data = results,
            //    TotalItems = totalItems,
            //    TotalPages = totalPages
            //};
        }

        /// <summary>
        /// Lấy chi tiết thông báo, bao gồm tên các bộ phận áp dụng, các file đính kèm của thông báo như ảnh, file excel, word,..
        /// </summary>
        public async Task<Entities.MemoNotification?> GetById(Guid Id)
        {
            //var query = _context.MemoNotifications.Where(e => e.Id == Id || e.ApplicationFormId == Id);

            //var result = await SelectMemoNotify(query)
            //    .Select(e => new Entities.MemoNotification
            //    {
            //        Id = e.Id,
            //        Title = e.Title,
            //        Content = e.Content,
            //        Priority = e.Priority,
            //        Status = e.Status,
            //        FromDate = e.FromDate,
            //        ToDate = e.ToDate,
            //        ApplyAllDepartment = e.ApplyAllDepartment,
            //        CreatedAt = e.CreatedAt,
            //        OrgUnit = e.OrgUnit,
            //        MemoNotificationDepartments = e.MemoNotificationDepartments,
            //        ApplicationForm = e.ApplicationForm,
            //        Files = _context.AttachFiles
            //            .Where(at => at.EntityId == e.Id && at.EntityType == nameof(Entities.MemoNotification))
            //            .Select(f => new Entities.File
            //            {
            //                Id = f.File != null ? f.File.Id : null,
            //                FileName = f.File != null ? f.File.FileName : null,
            //                ContentType = f.File != null ? f.File.ContentType : null
            //            }).ToList()
            //    }).FirstOrDefaultAsync();

            //return result;
            return null;
        }

        /// <summary>
        /// Tạo thông báo, có các trường hợp tạo thông báo
        /// General manager, công đoàn, manager của bộ phận, thành viên của bộ phận
        /// nếu như là general man, công đoàn, man của bộ phận tạo thông báo thì khi đó sẽ set trạng thái là FINAL_APPROVAL - để nhận biết là lần approval cuối
        /// còn thành viên trong bộ phận tạo thì có trạng thái là PENDING
        /// sau khi tạo xong thì sẽ gửi email cho người tiếp theo duyệt
        /// </summary>
        public async Task<object> Create(CreateMemoNotiRequest request, IFormFile[] files)
        {
            return null;
            //int? orgPositionId = request.OrgPositionId;
            //int? departmentId = request.DepartmentId;

            //if (departmentId == null)
            //{
            //    throw new ValidationException(Global.UserNotSetInformation);
            //}

            //int requestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION;

            //var userClaims = _httpContextAccessor.HttpContext?.User;
            //var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            //var isUnion = roleClaims?.Contains("UNION") ?? false;
            //var isGM = roleClaims?.Contains("GM") ?? false;

            //int? nextOrgPositionId = -1;
            //int statusApplicationForm = -1;

            //if (isGM)
            //{
            //    if (orgPositionId == null)
            //    {
            //        throw new ValidationException(Global.UserNotSetInformation);
            //    }
            //    nextOrgPositionId = orgPositionId;
            //    statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //}
            //else if (isUnion) //công đoàn
            //{
            //    var approvalFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.PositonContext == "ROLE_UNION");
            //    nextOrgPositionId = approvalFlow?.ToOrgPositionId;
            //    statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //}
            //else
            //{
            //    if (orgPositionId == null)
            //    {
            //        throw new ValidationException(Global.UserNotSetInformation);
            //    }

            //    var approvalFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.FromOrgPositionId == orgPositionId);

            //    if (approvalFlow != null && approvalFlow.IsFinal == true)
            //    {
            //        statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            //        nextOrgPositionId = approvalFlow?.ToOrgPositionId ?? orgPositionId;
            //    }
            //    else
            //    {
            //        var approvalFlowStaff = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.DepartmentId == departmentId);

            //        if (approvalFlowStaff != null)
            //        {
            //            statusApplicationForm = (int)StatusApplicationFormEnum.PENDING;
            //            nextOrgPositionId = approvalFlowStaff?.ToOrgPositionId ?? orgPositionId;
            //        }
            //        else
            //        {
            //            throw new NotFoundException(Global.NotFoundApprovalFlow);
            //        }
            //    }
            //}

            ////add new application form
            //var applicationForm = new ApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    UserCodeRequestor = request.UserCodeCreated,
            //    UserNameRequestor = request.CreatedBy,
            //    RequestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION,
            //    RequestStatusId = statusApplicationForm,
            //    CreatedAt = DateTimeOffset.Now,
            //    OrgPositionId = nextOrgPositionId,
            //    Code = Helper.GenerateFormCode("MNT"),
            //    UserCodeCreated = request.UserCodeCreated,
            //    UserNameCreated = request.CreatedBy,
            //    DepartmentId = departmentId
            //};

            //var memoNotify = new Entities.MemoNotification
            //{
            //    Id = Guid.NewGuid(),
            //    DepartmentId = departmentId,
            //    ApplicationFormId = applicationForm.Id,
            //    Title = request.Title,
            //    Content = request.Content,
            //    Status = request.Status,
            //    FromDate = request.FromDate,
            //    ToDate = request.ToDate,
            //    CreatedAt = DateTimeOffset.Now,
            //    ApplyAllDepartment = request.ApplyAllDepartment,
            //};

            //_context.ApplicationForms.Add(applicationForm);
            //_context.MemoNotifications.Add(memoNotify);

            ////memo department
            //if (request.ApplyAllDepartment == false)
            //{
            //    List<MemoNotificationDepartment> memoNotificationDepartments = [];

            //    if (request.DepartmentIdApply != null)
            //    {
            //        foreach (var item in request.DepartmentIdApply)
            //        {
            //            memoNotificationDepartments.Add(new MemoNotificationDepartment
            //            {
            //                MemoNotificationId = memoNotify.Id,
            //                DepartmentId = item
            //            });
            //        }
            //        _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
            //    }
            //}

            ////memo file
            //if (files.Length > 0)
            //{
            //    List<Domain.Entities.File> listEntityFiles = [];
            //    List<AttachFile> listAttachFiles = [];

            //    foreach (var file in files)
            //    {
            //        using var ms = new MemoryStream();
            //        await file.CopyToAsync(ms);

            //        var attach = new Domain.Entities.File
            //        {
            //            Id = Guid.NewGuid(),
            //            FileName = file.FileName,
            //            ContentType = file.ContentType,
            //            FileData = ms.ToArray(),
            //            CreatedAt = DateTimeOffset.Now
            //        };
            //        listEntityFiles.Add(attach);

            //        var attachFile = new AttachFile
            //        {
            //            FileId = attach.Id,
            //            EntityType = nameof(MemoNotification),
            //            EntityId = memoNotify.Id
            //        };

            //        listAttachFiles.Add(attachFile);
            //    }

            //    _context.Files.AddRange(listEntityFiles);
            //    _context.AttachFiles.AddRange(listAttachFiles);
            //}

            //await _context.SaveChangesAsync();

            //var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? 0);

            //string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-memo-notify/{memoNotify.Id}";

            //BackgroundJob.Enqueue<IEmailService>(job =>
            //    job.EmailSendMemoNotificationNeedApproval(
            //        receiveUser.Select(e => e.Email ?? "").ToList(),
            //        null,
            //        "Request for memo notification approval",
            //        TemplateEmail.EmailSendMemoNotificationNeedApproval(urlApproval),
            //        null,
            //        true
            //    )
            //);

            //return true;
        }

        //update thông báo
        public async Task<object> Update(Guid id, CreateMemoNotiRequest dto, IFormFile[] files)
        {
            //var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id || e.ApplicationFormId == id) ?? throw new NotFoundException("Memo notification not found!");

            //memoNotify.Title = dto.Title;
            //memoNotify.Content = dto.Content;
            //memoNotify.Status = dto.Status;
            //memoNotify.FromDate = dto.FromDate;
            //memoNotify.ToDate = dto.ToDate;
            //memoNotify.ApplyAllDepartment = dto.ApplyAllDepartment;
            //memoNotify.UpdatedAt = dto.UpdatedAt;

            //_context.MemoNotifications.Update(memoNotify);

            ////delete file
            //if (dto.DeleteFiles != null && dto.DeleteFiles.Count() > 0)
            //{
            //    var attachFiles = await _context.AttachFiles.Where(at => dto.DeleteFiles.Contains(at.FileId.ToString() ?? "")).ToListAsync();
            //    var fileDelete = await _context.Files.Where(f => dto.DeleteFiles.Contains(f.Id.ToString() ?? "")).ToListAsync();

            //    _context.AttachFiles.RemoveRange(attachFiles);
            //    _context.Files.RemoveRange(fileDelete);
            //}

            ////memo file
            //if (files.Length > 0)
            //{
            //    List<Domain.Entities.File> listEntityFiles = [];
            //    List<AttachFile> listAttachFiles = [];

            //    foreach (var file in files)
            //    {
            //        using var ms = new MemoryStream();
            //        await file.CopyToAsync(ms);

            //        var attach = new Domain.Entities.File
            //        {
            //            Id = Guid.NewGuid(),
            //            FileName = file.FileName,
            //            ContentType = file.ContentType,
            //            FileData = ms.ToArray(),
            //            CreatedAt = DateTimeOffset.Now
            //        };
            //        listEntityFiles.Add(attach);

            //        var attachfile = new AttachFile
            //        {
            //            FileId = attach.Id,
            //            EntityId = memoNotify.Id,
            //            EntityType = nameof(MemoNotification)
            //        };

            //        listAttachFiles.Add(attachfile);
            //    }

            //    _context.Files.AddRange(listEntityFiles);
            //    _context.AttachFiles.AddRange(listAttachFiles);
            //}

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
            //                DepartmentId = item
            //            });
            //        }
            //        _context.MemoNotificationDepartments.AddRange(memoNotificationDepartments);
            //    }
            //}

            //await _context.SaveChangesAsync();

            return true;
        }

        //delete thông báo, sẽ xóa nhưng dữ liên quan trước như là file, phòng ban vs thông báo rồi tới thông báo
        public async Task<object> Delete(Guid id)
        {
            //var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id || e.ApplicationFormId == id) ?? throw new NotFoundException("Notification not found to delete!");

            //await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == memoNotify.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            //await _context.ApplicationForms.Where(e => e.Id == memoNotify.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            //await _context.MemoNotifications.Where(e => e.Id == memoNotify.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        /// <summary>
        /// Lấy những thông báo được sẽ hiển thị ở ngoài màn hình chính homepage.
        /// thông báo phải có trạng thái được duyệt là complete và trạng thái hiển thị = true và nằm trong khoảng thời gian hiển thị
        /// những tbao áp dụng 1 vài phòng ban thì tìm kiếm thêm department_id của user, người nào tạo thông báo thì cũng sẽ hiển thị tbao cho người đó
        /// </summary>
        public async Task<List<MemoNotificationDto>> GetAllInHomePage(int? DepartmentId)
        {
            //var userCode = _httpContextAccessor.HttpContext?.User?.FindFirst("user_code")?.Value;

            //var today = TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")).Date;

            //var result = await _context.MemoNotifications
            //    .Include(e => e.ApplicationForm)
            //    .GroupJoin(
            //        _context.MemoNotificationDepartments,
            //        memo => memo.Id,
            //        memoDept => memoDept.MemoNotificationId,
            //        (memo, memoDeptGroup) => new { memo, memoDeptGroup }
            //    )
            //    .SelectMany(
            //        x => x.memoDeptGroup.DefaultIfEmpty(),
            //        (x, memoDept) => new { MemoNotification = x.memo, MemoNotificationDepartment = memoDept }
            //    )
            //    .Where(x =>
            //        x.MemoNotification.Status == true &&
            //        x.MemoNotification.ApplicationForm != null &&
            //        x.MemoNotification.ApplicationForm.RequestStatusId == (int)StatusApplicationFormEnum.COMPLETE &&
            //        x.MemoNotification.FromDate.HasValue && x.MemoNotification.FromDate.Value.Date <= today &&
            //        x.MemoNotification.ToDate.HasValue && x.MemoNotification.ToDate.Value.Date >= today &&
            //        (
            //            DepartmentId == GM_Department ||
            //            x.MemoNotification.ApplyAllDepartment == true ||
            //            x.MemoNotificationDepartment != null &&
            //            (x.MemoNotificationDepartment.DepartmentId == DepartmentId || x.MemoNotification.ApplicationForm.UserCodeCreated == userCode)
            //        )
            //    )
            //    .Select(x => x.MemoNotification)
            //    .Distinct()
            //    .ToListAsync();

            //return MemoNotifyMapper.ToDtoList(result);
            return null;
        }

        //tìm kiếm file để trả về file cần download
        public async Task<Domain.Entities.File> GetFileDownload(Guid id)
        {
            var file = await _context.Files.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo Notification not found!");

            return file;
        }

        /// <summary>
        /// Hàm phê duyệt tbao, sẽ tạo 1 bản ghi vào tbl history_applications_form để lưu lịch sử đã phê duyệt như là approval or reject, ai duyệt,tgian duyệt,..
        /// tìm kiếm luồng duyệt tiếp theo dựa trên orgUnitId trong bảng work_flow_steps
        /// nếu như request có status = false thì là reject, ngược lại là approved
        /// nếu reject hoặc approval bước cuối cùng thì gửi email cho người tạo thông báo, nếu approval không phải cuối thì gửi email cho người tiếp theo duyệt
        /// </summary>
        public async Task<object> Approval(ApprovalRequest request)
        {
            //var orgPositionId = request.OrgPositionId;

            //var memoNotify = await _context.MemoNotifications.Include(e => e.ApplicationForm).FirstOrDefaultAsync(e => e.Id == request.MemoNotificationId || e.ApplicationFormId == request.MemoNotificationId);

            //if (memoNotify == null)
            //{
            //    throw new NotFoundException("Memo notify not found");
            //}

            //var applicationForm = memoNotify.ApplicationForm;

            ////nếu không tìm thấy
            //if (applicationForm == null)
            //{
            //    throw new NotFoundException("Application form memo notify not found");
            //}

            ////nếu như đơn này hiện tại k phải người này duyệt
            //if (applicationForm.OrgPositionId != request.OrgPositionId)
            //{
            //    throw new ValidationException(Global.NotPermissionApproval);
            //}

            ////nếu như là vị trí người này duyệt nhưng trạng thái đã là complete hoặc reject => đã có người approval
            //if (applicationForm.OrgPositionId == request.OrgPositionId &&
            //    (
            //        applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.COMPLETE ||
            //        applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.REJECT
            //    )
            //)
            //{
            //    throw new ValidationException(Global.HasBeenApproval);
            //}

            //applicationForm.Id = applicationForm.Id;

            //var historyApplicationForm = new HistoryApplicationForm
            //{
            //    Id = Guid.NewGuid(),
            //    ApplicationFormId = applicationForm?.Id,
            //    UserNameApproval = request.UserNameApproval,
            //    UserCodeApproval = request.UserCodeApproval,
            //    Note = request.Note,
            //    Action = request.Status == true ? "APPROVAL" : "REJECT",
            //    CreatedAt = DateTimeOffset.Now
            //};

            //int? nextOrgPositionId = -1;
            //bool isFinal = false;

            ////reject
            //if (request.Status == false)
            //{
            //    applicationForm!.RequestStatusId = (int)StatusApplicationFormEnum.REJECT;
            //}
            //else //approval
            //{
            //    if (applicationForm!.RequestStatusId == (int)StatusApplicationFormEnum.FINAL_APPROVAL) //if is final -> approval will publish
            //    {
            //        applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.COMPLETE;
            //        isFinal = true;
            //    }
            //    else
            //    {
            //        var workFlowStep = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION && e.FromOrgPositionId == orgPositionId);

            //        if (workFlowStep != null)
            //        {
            //            applicationForm.RequestStatusId = workFlowStep.IsFinal == true ? (int)StatusApplicationFormEnum.FINAL_APPROVAL : (int)StatusApplicationFormEnum.IN_PROCESS;
            //            nextOrgPositionId = workFlowStep?.ToOrgPositionId ?? orgPositionId;
            //        }
            //        else
            //        {
            //            throw new NotFoundException(Global.NotFoundApprovalFlow);
            //        }
            //    }
            //}

            //applicationForm.OrgPositionId = nextOrgPositionId;

            //_context.HistoryApplicationForms.Add(historyApplicationForm);

            //_context.ApplicationForms.Update(applicationForm);

            //await _context.SaveChangesAsync();

            //if (request.Status == false || request.Status == true && isFinal) //gửi email cho người tạo thông báo là complete or reject
            //{
            //    var userRequest = await _context.Users.Where(e => memoNotify.ApplicationForm != null && e.UserCode == memoNotify.ApplicationForm.UserCodeCreated).ToListAsync();

            //    bool isApproved = request.Status == true;
            //    string title = isApproved ? "approved" : "reject";

            //    string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-memo-notify/{memoNotify.Id}";

            //    BackgroundJob.Enqueue<IEmailService>(job =>
            //        job.EmailSendMemoNotificationHasBeenCompletedOrReject(
            //            userRequest.Select(e => e.Email ?? "").ToList(),
            //            null,
            //            $"Your request memo notification has been {title}",
            //            TemplateEmail.EmailSendMemoNotificationHasBeenCompletedOrReject(urlView, isApproved),
            //            null,
            //            true
            //        )
            //    );
            //}
            //else //gửi cho người tiếp theo duyệt
            //{
            //    var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? 0);

            //    string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-memo-notify/{memoNotify.Id}";

            //    BackgroundJob.Enqueue<IEmailService>(job =>
            //        job.EmailSendMemoNotificationNeedApproval(
            //            receiveUser.Select(e => e.Email ?? "").ToList(),
            //            null,
            //            "Request for memo notification approval",
            //            TemplateEmail.EmailSendMemoNotificationNeedApproval(urlApproval),
            //            null,
            //            true
            //        )
            //    );
            //}

            //return true;
            return null;
        }

        private static IQueryable<Entities.MemoNotification> SelectMemoNotify(IQueryable<Entities.MemoNotification> query, bool? IsGetFileRelation = null)
        {
            //return query.Select(e => new Entities.MemoNotification
            //{
            //    Id = e.Id,
            //    Title = e.Title,
            //    Content = e.Content,
            //    Priority = e.Priority,
            //    Status = e.Status,
            //    FromDate = e.FromDate,
            //    ToDate = e.ToDate,
            //    ApplyAllDepartment = e.ApplyAllDepartment,
            //    CreatedAt = e.CreatedAt,

            //    MemoNotificationDepartments = e.MemoNotificationDepartments.Select(d => new Entities.MemoNotificationDepartment
            //    {
            //        Id = d.Id,
            //        MemoNotificationId = d.MemoNotificationId,
            //        DepartmentId = d.DepartmentId,
            //        OrgUnit = d.OrgUnit == null ? null : new Entities.OrgUnit
            //        {
            //            Id = d.OrgUnit.Id,
            //            Name = d.OrgUnit.Name
            //        }
            //    }).ToList(),

            //    OrgUnit = e.OrgUnit,

            //    ApplicationForm = e.ApplicationForm == null ? null : new ApplicationForm
            //    {
            //        Id = e.ApplicationForm.Id,
            //        RequestStatusId = e.ApplicationForm.RequestStatusId,
            //        RequestTypeId = e.ApplicationForm.RequestTypeId,
            //        Code = e.ApplicationForm.Code,
            //        UserCodeRequestor = e.ApplicationForm.UserCodeRequestor,
            //        UserNameRequestor = e.ApplicationForm.UserNameRequestor,
            //        UserCodeCreated = e.ApplicationForm.UserCodeCreated,
            //        UserNameCreated = e.ApplicationForm.UserNameCreated,

            //        RequestStatus = e.ApplicationForm.RequestStatus == null ? null : new RequestStatus
            //        {
            //            Id = e.ApplicationForm.RequestStatus.Id,
            //            Name = e.ApplicationForm.RequestStatus.Name,
            //            NameE = e.ApplicationForm.RequestStatus.NameE,
            //        },

            //        RequestType = e.ApplicationForm.RequestType == null ? null : new Entities.RequestType
            //        {
            //            Id = e.ApplicationForm.RequestType.Id,
            //            Name = e.ApplicationForm.RequestType.Name,
            //            NameE = e.ApplicationForm.RequestType.NameE,
            //        },

            //        HistoryApplicationForms = e.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).Select(h => new Entities.HistoryApplicationForm
            //        {
            //            Id = h.Id,
            //            UserCodeApproval = h.UserCodeApproval,
            //            UserNameApproval = h.UserNameApproval,
            //            Note = h.Note,
            //            Action = h.Action,
            //            ApplicationFormId = h.ApplicationFormId,
            //            CreatedAt = h.CreatedAt,
            //        }).ToList()

            //    }
            //});
            return null;
        }
    }
}
