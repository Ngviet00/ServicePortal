using System.Data;
using System.Security.Claims;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MemoNotification;
using ServicePortals.Application.Dtos.MemoNotification.Requests;
using ServicePortals.Application.Dtos.MemoNotification.Responses;
using ServicePortals.Application.Interfaces.MemoNotification;
using ServicePortals.Application.Interfaces.OrgUnit;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrgUnitService _orgService;
        private readonly IUserService _userService;

        public MemoNotificationService(
            ApplicationDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            IOrgUnitService orgService,
            IUserService userService
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _orgService = orgService;
            _userService = userService;
        }

        /// <summary>
        /// Lấy danh sách thông báo đã tạo của user, join với tblBoPhan ở bên viclock để lấy tên
        /// vì có 1 số thông báo chỉ áp dụng cho 1 vài bộ phận/phòng ban
        /// </summary>
        public async Task<PagedResults<GetAllMemoNotifyResponse>> GetAll(GetAllMemoNotiRequest request)
        {
            int pageSize = request.PageSize;
            int page = request.Page;
            string? userCode = request.CurrentUserCode;

            var q = _context.MemoNotifications.AsQueryable();

            q = q.Where(e => e.UserCodeCreated == userCode);

            var totalItems = await q.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var query = await _context.MemoNotifications
                .Where(e => e.UserCodeCreated == userCode)
                .OrderByDescending(e => e.CreatedAt)
                .Select(e => new GetAllMemoNotifyResponse
                {
                    Id = e.Id,
                    Title = e.Title,
                    Content = e.Content,
                    ApplyAllDepartment = e.ApplyAllDepartment,
                    FromDate = e.FromDate,
                    ToDate = e.ToDate,
                    Status = e.Status,
                    UserCodeCreated = e.UserCodeCreated,
                    CreatedAt = e.CreatedAt,
                    CreatedBy = e.CreatedBy,
                    Priority = e.Priority,
                    ApplicationForm = new ApplicationForm
                    {
                        Id = e.ApplicationForm != null ? e.ApplicationForm.Id : null,
                        RequestStatusId = e.ApplicationForm != null ? e.ApplicationForm.RequestStatusId : null,
                        HistoryApplicationForms = e.ApplicationForm != null ? e.ApplicationForm.HistoryApplicationForms.OrderByDescending(h => h.CreatedAt).Take(1).ToList() : new List<HistoryApplicationForm>()
                    },
                    MemoNotificationDepartments = string.Join(", ", 
                        e.MemoNotificationDepartments
                            .Where(md => md.OrgUnit != null && md.OrgUnit.UnitId == (int)UnitEnum.Phong)
                            .Select(md => md.OrgUnit != null ? md.OrgUnit.Name : "")
                            .ToList()
                    )
                })
                .Skip(((page - 1) * pageSize))
                .Take(pageSize)
                .ToListAsync();

            return new PagedResults<GetAllMemoNotifyResponse>
            {
                Data = query,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Lấy chi tiết thông báo, bao gồm tên các bộ phận áp dụng, các file đính kèm của thông báo như ảnh, file excel, word,..
        /// </summary>
        public async Task<MemoNotificationDto> GetById(Guid id)
        {
            var memoNotify = await _context.MemoNotifications
                .Include(e => e.ApplicationForm)
                    .ThenInclude(e => e.HistoryApplicationForms)
                .FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo Notification not found!");

            var departmentIds = await _context.MemoNotificationDepartments
                .Where(mnd => mnd.MemoNotificationId == id)
                .Select(mnd => mnd.DepartmentId)
                .ToArrayAsync();

            var allDepartments = await _orgService.GetAllDepartments();
            var nameDepartmentApplies = allDepartments.Where(e => departmentIds.Contains(e.DeptId)).Select(e => e.Name).ToList();

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
                Files = files,
                RequestTypeId = memoNotify?.ApplicationForm?.RequestTypeId,
                HistoryApplicationForm = memoNotify?.ApplicationForm?.HistoryApplicationForms
                    .OrderByDescending(h => h.CreatedAt)
                    .Select(x => new HistoryApplicationForm
                    {
                        Id = x.Id,
                        UserApproval =  x.UserApproval,
                        UserCodeApproval =  x.UserCodeApproval,
                        ActionType = x.ActionType,
                        Comment = x.Comment,
                        CreatedAt = x.CreatedAt
                    })
                    .FirstOrDefault()
            };

            result.DepartmentNames = string.Join(", ", nameDepartmentApplies);

            return result;
        }

        /// <summary>
        /// Tạo thông báo, có các trường hợp tạo thông báo
        /// General manager, công đoàn, manager của bộ phận, thành viên của bộ phận
        /// nếu như là general man, công đoàn, man của bộ phận tạo thông báo thì khi đó sẽ set trạng thái là FINAL_APPROVAL - để nhận biết là lần approval cuối
        /// còn thành viên trong bộ phận tạo thì có trạng thái là PENDING
        /// sau khi tạo xong thì sẽ gửi email cho người tiếp theo duyệt
        /// </summary>
        public async Task<MemoNotificationDto> Create(CreateMemoNotiRequest request, IFormFile[] files)
        {
            int? orgUnitId = request.OrgUnitId;
            int? departmentId = request.DepartmentId;

            if (departmentId == null)
            {
                throw new ValidationException("Thông tin vị trí chưa được cập nhật, liên hệ với bộ phận HR");
            }

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
                DepartmentId = request.DepartmentId,
                ApplicationFormId = applicationForm.Id,
                Code = Helper.GenerateFormCode("MNT"),
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

        //update thông báo
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
                var attachFiles = await _context.AttachFiles.Where(at => dto.DeleteFiles.Contains(at.FileId.ToString() ?? "")).ToListAsync();
                var fileDelete = await _context.Files.Where(f => dto.DeleteFiles.Contains(f.Id.ToString() ?? "")).ToListAsync();

                _context.AttachFiles.RemoveRange(attachFiles);
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

        //delete thông báo, sẽ xóa nhưng dữ liên quan trước như là file, phòng ban vs thông báo rồi tới thông báo
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

        /// <summary>
        /// Lấy những thông báo được sẽ hiển thị ở ngoài màn hình chính homepage.
        /// thông báo phải có trạng thái được duyệt là complete và trạng thái hiển thị = true và nằm trong khoảng thời gian hiển thị
        /// những tbao áp dụng 1 vài phòng ban thì tìm kiếm thêm department_id của user, người nào tạo thông báo thì cũng sẽ hiển thị tbao cho người đó
        /// </summary>
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
            var orgUnitId = request.OrgUnitId;

            var memoNotify = await _context.MemoNotifications.Include(e => e.ApplicationForm).FirstOrDefaultAsync(e => e.Id == request.MemoNotificationId);

            if (memoNotify == null)
            {
                throw new NotFoundException("Memo notify not found");
            }

            var applicationForm = memoNotify.ApplicationForm;

            //nếu không tìm thấy
            if (applicationForm == null)
            {
                throw new NotFoundException("Application form memo notify not found");
            }

            //nếu như đơn này hiện tại k phải người này duyệt
            if (applicationForm.CurrentOrgUnitId != request.OrgUnitId)
            {
                throw new ValidationException("Memo notify has been approval, reload page!");
            }

            //nếu như là vị trí người này duyệt nhưng trạng thái đã là complete hoặc reject => đã có người approval
            if (applicationForm.CurrentOrgUnitId == request.OrgUnitId &&
                (applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.COMPLETE || applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.REJECT)
            )
            {
                throw new ValidationException("Memo notify has been approval, reload page!");
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
    }
}
