using System.Data;
using System.Security.Claims;
using Dapper;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
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
using ServicePortals.Shared.Exceptions;
using Entities = ServicePortals.Domain.Entities;
using Z.EntityFramework.Plus;

namespace ServicePortals.Application.Services.MemoNotification
{
    public class MemoNotificationService : IMemoNotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IOrgPositionService _orgPositionService;

        public MemoNotificationService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService,
            IConfiguration configuration,
            IOrgPositionService orgPositionService
        )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _configuration = configuration;
            _orgPositionService = orgPositionService;
        }

        public async Task<PagedResults<GetAllMemoNotifyResponse>> GetAll(GetAllMemoNotificationRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<GetAllMemoNotifyResponse>(
                    "dbo.MemoNotification_GET_GetMemoNotificationByUserCode",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<GetAllMemoNotifyResponse>
            {
                Data = (List<GetAllMemoNotifyResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        /// <summary>
        /// Lấy chi tiết thông báo, bao gồm tên các bộ phận áp dụng, các file đính kèm của thông báo như ảnh, file excel, word,..
        /// </summary>
        public async Task<Entities.MemoNotification?> GetById(Guid Id)
        {
            var memoNotification = await _context.MemoNotifications
                .Include(e => e.OrgUnit)
                .Include(e => e.ApplicationFormItem)
                .Include(e => e.MemoNotificationDepartments)
                    .ThenInclude(e => e.OrgUnit)
                .FirstOrDefaultAsync(
                    e => e.Id == Id || 
                        (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == Id)
                );

            if (memoNotification == null)
            {
                throw new NotFoundException("Memo notification not found, please check again");
            }

            var applicationForm = await _context.ApplicationForms
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .Include(e => e.AssignedTasks)
                .FirstOrDefaultAsync(e =>
                    memoNotification != null &&
                    memoNotification.ApplicationFormItem != null && 
                    e.Id == memoNotification.ApplicationFormItem.ApplicationFormId
                );

            var files = await _context.AttachFiles
                .Where(at => at.EntityId == memoNotification!.Id && at.EntityType == nameof(Entities.MemoNotification))
                .Select(f => new Entities.File
                {
                    Id = f.File != null ? f.File.Id : null,
                    FileName = f.File != null ? f.File.FileName : null,
                    ContentType = f.File != null ? f.File.ContentType : null
                })
                .ToListAsync();

            memoNotification!.Files = files;

            if (memoNotification.ApplicationFormItem != null)
            {
                memoNotification.ApplicationFormItem.MemoNotifications = [];
            }

            foreach (var item in memoNotification.MemoNotificationDepartments)
            {
                item.MemoNotifications = null;
            }

            if (applicationForm != null)
            {
                var historyApplicationForms = await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == applicationForm.Id).OrderByDescending(e => e.ActionAt).ToListAsync();

                applicationForm.HistoryApplicationForms = historyApplicationForms;
                memoNotification!.ApplicationFormItem!.ApplicationForm = applicationForm;
            }

            return memoNotification;
        }

        /// <summary>
        /// Tạo thông báo, có các trường hợp tạo thông báo
        /// General manager, công đoàn, manager của bộ phận, thành viên của bộ phận
        /// nếu như là general man, công đoàn, man của bộ phận tạo thông báo thì khi đó sẽ set trạng thái là FINAL_APPROVAL - để nhận biết là lần approval cuối
        /// còn thành viên trong bộ phận tạo thì có trạng thái là PENDING
        /// sau khi tạo xong thì sẽ gửi email cho người tiếp theo duyệt
        /// </summary>
        public async Task<object> Create(CreateMemoNotificationRequest request, IFormFile[] files)
        {
            int? orgPositionId = request.OrgPositionId;
            int? departmentId = request.DepartmentId ?? throw new ValidationException(Global.UserNotSetInformation);

            int requestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION;

            var userClaims = _httpContextAccessor.HttpContext?.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var isUnion = roleClaims?.Contains("UNION") ?? false;
            var isGM = roleClaims?.Contains("GM") ?? false;

            int? nextOrgPositionId = -1;
            int statusApplicationForm = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId);

            if (isGM)
            {
                nextOrgPositionId = orgPositionId ?? throw new ValidationException(Global.UserNotSetInformation);
                statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
            }
            else if (isUnion) //công đoàn
            {
                var approvalFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.PositonContext == "ROLE_UNION");
                nextOrgPositionId = approvalFlow?.ToOrgPositionId;
                
                if (approvalFlow?.IsFinal == true)
                {
                    statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                }
            }
            else
            {
                if (orgPosition == null)
                {
                    throw new ValidationException(Global.UserNotSetInformation);
                }

                if (orgPosition.UnitId == (int)UnitEnum.Manager)
                {
                    var approvalFlowManager = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == requestTypeId && e.PositonContext == "MANAGER");
                    nextOrgPositionId = approvalFlowManager?.ToOrgPositionId;

                    if (approvalFlowManager?.IsFinal == true)
                    {
                        statusApplicationForm = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var managerOrgPositionOfStaff = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPosition.Id ?? -1);
                    nextOrgPositionId = managerOrgPositionOfStaff?.Id ?? -1;
                }
            }

            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                Code = Helper.GenerateFormCode("MNT"),
                RequestTypeId = (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION,
                RequestStatusId = statusApplicationForm,
                DepartmentId = departmentId,
                OrgPositionId = nextOrgPositionId,
                UserCodeCreatedBy = request.UserCodeCreated,
                CreatedBy = request.CreatedBy,
                CreatedAt = DateTimeOffset.Now,
            };

            var historyApplicationForm = new HistoryApplicationForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                Action = "Created",
                ActionBy = request?.CreatedBy,
                UserCodeAction = request?.UserCodeCreated,
                ActionAt = DateTimeOffset.Now,
            };

            var applicationFormItem = new ApplicationFormItem
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                UserCode = request?.UserCodeCreated,
                UserName = request?.CreatedBy,
                Status = true,
                CreatedAt = DateTimeOffset.Now
            };

            var memoNotify = new Entities.MemoNotification
            {
                Id = Guid.NewGuid(),
                DepartmentId = departmentId,
                ApplicationFormItemId = applicationFormItem.Id,
                Title = request?.Title,
                Content = request?.Content,
                Status = request?.Status,
                FromDate = request?.FromDate,
                ToDate = request?.ToDate,
                CreatedAt = DateTimeOffset.Now,
                ApplyAllDepartment = request?.ApplyAllDepartment,
            };

            _context.ApplicationForms.Add(applicationForm);
            _context.HistoryApplicationForms.Add(historyApplicationForm);
            _context.ApplicationFormItems.Add(applicationFormItem);
            _context.MemoNotifications.Add(memoNotify);

            //memo department
            if (request?.ApplyAllDepartment == false)
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
                List<Entities.File> listEntityFiles = [];
                List<AttachFile> listAttachFiles = [];

                foreach (var file in files)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms);

                    var attach = new Entities.File
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

            await _context.SaveChangesAsync();

            var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? 0);

            string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-memo-notify/{memoNotify.Id}";

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.EmailSendMemoNotification(
                    receiveUser.Select(e => e.Email ?? "").ToList(),
                    null,
                    "Request for memo notification approval",
                    TemplateEmail.SendContentEmail("Request for memo notification approval", urlApproval, applicationForm.Code),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Update(Guid id, CreateMemoNotificationRequest dto, IFormFile[] files)
        {
            var memoNotify = await _context.MemoNotifications.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Memo notification not found!");

            memoNotify.Title = dto.Title;
            memoNotify.Content = dto.Content;
            memoNotify.Status = dto.Status;
            memoNotify.FromDate = dto.FromDate;
            memoNotify.ToDate = dto.ToDate;
            memoNotify.ApplyAllDepartment = dto.ApplyAllDepartment;
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

            return true;
        }

        public async Task<object> Delete(Guid id)
        {
            var memoNotify = await _context.MemoNotifications.Include(e => e.ApplicationFormItem).FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Notification not found to delete!");

            var applicationFormId = memoNotify.ApplicationFormItem?.ApplicationFormId;

            await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == applicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationForms.Where(e => e.Id == applicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationFormItems.Where(e => e.Id == memoNotify.ApplicationFormItemId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.MemoNotifications.Where(e => e.Id == memoNotify.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        /// <summary>
        /// Lấy những thông báo được sẽ hiển thị ở ngoài màn hình chính homepage.
        /// thông báo phải có trạng thái được duyệt là complete và trạng thái hiển thị = true và nằm trong khoảng thời gian hiển thị
        /// những tbao áp dụng 1 vài phòng ban thì tìm kiếm thêm department_id của user, người nào tạo thông báo thì cũng sẽ hiển thị tbao cho người đó
        /// </summary>
        public async Task<List<GetAllMemoNotifyResponse>> GetAllInHomePage(int? DepartmentId)
        {
            var userCode = _httpContextAccessor.HttpContext?.User?.FindFirst("user_code")?.Value;

            var parameters = new DynamicParameters();

            parameters.Add("@Page", 1, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", 10, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCodeCreated", userCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@DepartmentId", DepartmentId, DbType.String, ParameterDirection.Input);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<GetAllMemoNotifyResponse>(
                    "dbo.MemoNotification_GET_GetAllInHomePage",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            return (List<GetAllMemoNotifyResponse>)results;
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
            var orgPositionId = request.OrgPositionId;

            var memoNotify = await _context.MemoNotifications
                .Include(e => e.ApplicationFormItem)
                .FirstOrDefaultAsync(e => e.Id == request.MemoNotificationId || (e.ApplicationFormItem != null && e.ApplicationFormItem.ApplicationFormId == request.MemoNotificationId))
                ?? throw new NotFoundException("Memo notify not found");

            var applicationForm = await _context.ApplicationForms.FirstOrDefaultAsync(e => memoNotify.ApplicationFormItem != null && e.Id == memoNotify.ApplicationFormItem.ApplicationFormId);

            //nếu không tìm thấy
            if (applicationForm == null)
            {
                throw new NotFoundException("Application form memo notify not found");
            }

            //nếu như đơn này hiện tại k phải người này duyệt
            if (applicationForm.OrgPositionId != request.OrgPositionId)
            {
                throw new ValidationException(Global.NotPermissionApproval);
            }

            //nếu như là vị trí người này duyệt nhưng trạng thái đã là complete hoặc reject => đã có người approval
            if (applicationForm.OrgPositionId == request.OrgPositionId &&
                (
                    applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.COMPLETE ||
                    applicationForm.RequestStatusId == (int)StatusApplicationFormEnum.REJECT
                )
            )
            {
                throw new ValidationException(Global.HasBeenApproval);
            }

            applicationForm.Id = applicationForm.Id;

            var historyApplicationForm = new HistoryApplicationForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm?.Id,
                ActionBy = request.UserNameApproval,
                UserCodeAction = request.UserCodeApproval,
                Note = request.Note,
                Action = request.Status == true ? "Approved" : "Reject",
                ActionAt = DateTimeOffset.Now
            };

            int? nextOrgPositionId = -1;
            
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
                    var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == orgPositionId) ?? throw new ValidationException(Global.UserNotSetInformation);

                    if (orgPosition.UnitId == (int)UnitEnum.Manager)
                    {
                        var approvalFlowManager = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.CREATE_MEMO_NOTIFICATION && e.PositonContext == "MANAGER");
                        nextOrgPositionId = approvalFlowManager?.ToOrgPositionId;

                        if (approvalFlowManager?.IsFinal == true)
                        {
                            applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                        }
                    }
                    else
                    {
                        var managerOrgPositionOfStaff = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(orgPositionId ?? -1);
                        nextOrgPositionId = managerOrgPositionOfStaff?.Id ?? -1;
                        applicationForm.RequestStatusId = (int)StatusApplicationFormEnum.IN_PROCESS;
                    }
                }
            }

            applicationForm.OrgPositionId = nextOrgPositionId;

            _context.HistoryApplicationForms.Add(historyApplicationForm);
            _context.ApplicationForms.Update(applicationForm);
            await _context.SaveChangesAsync();

            if (request.Status == false || request.Status == true && isFinal) //gửi email cho người tạo thông báo là complete or reject
            {
                var userRequest = await _context.Users.Where(e => e.UserCode == applicationForm.UserCodeCreatedBy).ToListAsync();

                bool isApproved = request.Status == true;
                string title = isApproved ? "approved" : "reject";

                string urlView = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/view-memo-notify/{memoNotify.Id}";

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.EmailSendMemoNotification(
                        userRequest.Select(e => e.Email ?? "").ToList(),
                        null,
                        $"Your request memo notification has been {title}",
                        TemplateEmail.SendContentEmail($"Your request memo notification has been {title}", urlView, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );
            }
            else //gửi cho người tiếp theo duyệt
            {
                var receiveUser = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? 0);

                string urlApproval = $@"{_configuration["Setting:UrlFrontEnd"]}/approval/approval-memo-notify/{memoNotify.Id}";

                BackgroundJob.Enqueue<IEmailService>(job =>
                    job.EmailSendMemoNotification(
                        receiveUser.Select(e => e.Email ?? "").ToList(),
                        null,
                        "Request for memo notification approval",
                        TemplateEmail.SendContentEmail($"Request for memo notification approval", urlApproval, applicationForm.Code ?? ""),
                        null,
                        true
                    )
                );
            }

            return true;
        }
    }
}
