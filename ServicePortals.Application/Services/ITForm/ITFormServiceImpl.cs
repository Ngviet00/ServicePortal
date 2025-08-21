using System.Text.Json;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Common;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Application.Mappers;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Email;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITFormServiceImpl : Interfaces.ITForm.ITFormService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IOrgPositionService _orgPositionService;
        public ITFormServiceImpl(ApplicationDbContext context, IUserService userService, IOrgPositionService orgPositionService)
        {
            _context = context;
            _userService = userService;
            _orgPositionService = orgPositionService;
        }

        public async Task<PagedResults<ITFormResponse>> GetAll(GetAllITFormRequest request)
        {
            string? userCode = request.UserCode;
            int page = request.Page;
            int pageSize = request.PageSize;

            if (string.IsNullOrWhiteSpace(userCode))
            {
                throw new ValidationException("UserCode is required");
            }

            var query = _context.ITForms.Where(e => (e.UserCodeRequestor == userCode || e.UserCodeCreated == userCode) && e.DeletedAt == null).AsQueryable();

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var result = await query
                .Include(x => x.Priority)
                .Include(x => x.OrgUnit)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(haf => haf.HistoryApplicationForms)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(af => af.RequestStatus)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(af => af.RequestType)
                .Include(x => x.ItFormCategories)
                    .ThenInclude(ift => ift.ITCategory)
                .Skip(((page - 1) * pageSize)).Take(pageSize)
                .ToListAsync();

            var resultDtos = ITFormMapper.ToDtoList(result);

            return new PagedResults<ITFormResponse>
            {
                Data = resultDtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<ITFormResponse?> GetById(Guid Id)
        {
            var result = await _context.ITForms
                .Include(x => x.Priority)
                .Include(x => x.OrgUnit)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(haf => haf.HistoryApplicationForms)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(af => af.RequestStatus)
                .Include(x => x.ApplicationForm)
                    .ThenInclude(af => af.RequestType)
                .Include(x => x.ItFormCategories)
                    .ThenInclude(ift => ift.ITCategory)
                .FirstOrDefaultAsync(e => e.Id == Id && e.DeletedAt == null);

            var resultDto = ITFormMapper.ToDto(result);

            return resultDto;
        }

        public async Task<object> Create(CreateITFormRequest request)
        {
            if (request.DepartmentId == null || request.OrgPositionId <= 0)
            {
                throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");
            }

            int? nextOrgPositionId = 0;

            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            //nếu có custom approval flow
            if (approvalFlowCurrentPositionId != null)
            {
                nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId;
                
                if (approvalFlowCurrentPositionId.IsFinal == true)
                {
                    status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                }
            }
            else
            {
                //manager các bộ phận
                if (orgPosition?.ParentOrgPositionId == null)
                {
                    var approvalFlows = await _context.ApprovalFlows.Where(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.PositonContext == "MANAGER").ToListAsync();

                    //nếu như đơn có các trạng thái như Server,... thì cần qua sếp trên, nếu k thì qua luôn manager IT
                    var finalStep = approvalFlows
                        .FirstOrDefault(step => !string.IsNullOrEmpty(step.Condition) &&
                            JsonDocument.Parse(step.Condition)
                                .RootElement.GetProperty("it_category")
                                .EnumerateArray()
                                .Select(e => e.GetProperty("id").GetInt32())
                                .Any(id => request.ITCategories.Contains(id)))
                        ?? approvalFlows.FirstOrDefault(step => step.Condition == null);

                    nextOrgPositionId = finalStep?.ToOrgPositionId;

                    if (finalStep?.IsFinal == true)
                    {
                        status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
                    }
                }
                else
                {
                    var getManagerOrgPostionId = await _orgPositionService.GetManagerOrgPostionIdByOrgPositionId(request.OrgPositionId);
                    nextOrgPositionId = getManagerOrgPostionId?.Id;
                }
            }

            var applicationForm = new ApplicationForm
            {
                Id = Guid.NewGuid(),
                UserCodeRequestor = request.UserCodeRequestor,
                RequestTypeId = (int)RequestTypeEnum.FORM_IT,
                OrgPositionId = nextOrgPositionId,
                RequestStatusId = status,
                CreatedAt = DateTimeOffset.Now,
            };

            var formIT = new Domain.Entities.ITForm
            {
                Id = Guid.NewGuid(),
                ApplicationFormId = applicationForm.Id,
                Code = Helper.GenerateFormCode("FIT"),
                UserCodeRequestor = request.UserCodeRequestor,
                UserNameRequestor = request.UserNameRequestor,
                UserCodeCreated = request.UserCodeCreated,
                UserNameCreated = request.UserNameCreated,
                DepartmentId = request.DepartmentId,
                Email = request.Email,
                Position = request.Position,
                PriorityId = request.PriorityId,
                Reason = request.Reason,
                RequestDate = request.RequestDate,
                RequiredCompletionDate = request.RequiredCompletionDate,
                CreatedAt = DateTimeOffset.Now
            };

            List<ITFormCategory> itFormCategories = [];

            foreach (var item in request.ITCategories)
            {
                itFormCategories.Add(new ITFormCategory
                {
                    ITFormId = formIT.Id,
                    ITCategoryId = item
                });
            }

            _context.ApplicationForms.Add(applicationForm);
            _context.ITForms.Add(formIT);
            _context.ITFormCategories.AddRange(itFormCategories);

            await _context.SaveChangesAsync();

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            BackgroundJob.Enqueue<IEmailService>(job =>
                job.SendEmailFormIT(
                    nextUserInfo.Select(e => e.Email ?? "").ToList(),
                    null,
                    "New approval request form IT",
                    TemplateEmail.EmailFormIT(),
                    null,
                    true
                )
            );

            return true;
        }

        public async Task<object> Delete(Guid Id)
        {
            var item = await _context.ITForms.FirstOrDefaultAsync(e => e.Id == Id && e.DeletedAt == null) ?? throw new NotFoundException("Form IT not found");

            await _context.HistoryApplicationForms.Where(e => e.ApplicationFormId == item.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ApplicationForms.Where(e => e.Id == item.ApplicationFormId).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            await _context.ITForms.Where(e => e.Id == item.Id).ExecuteUpdateAsync(s => s.SetProperty(e => e.DeletedAt, DateTimeOffset.Now));

            return true;
        }

        public async Task<object> Update(Guid Id, UpdateITFormRequest request)
        {
            var item = await _context.ITForms.FirstOrDefaultAsync(e => e.Id == Id && e.DeletedAt == null) ?? throw new NotFoundException("Form IT not found");

            item.Position = request.Position;
            item.Email = request.Email;
            item.RequestDate = request.RequestDate;
            item.RequiredCompletionDate = request.RequiredCompletionDate;
            item.Reason = request.Reason;
            item.PriorityId = request.PriorityId;

            _context.ITForms.Update(item);

            await _context.SaveChangesAsync();

            return true;
        }

        public Task<object> Approval(ApprovalRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
