using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Application.Interfaces.User;
using ServicePortals.Domain.Entities;
using ServicePortals.Domain.Enums;
using ServicePortals.Infrastructure.Data;
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

        public Task<object> Approval(ApprovalRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<object> Create(CreateITFormRequest request)
        {
            if (request.DepartmentId == null || request.OrgPositionId <= 0) // || orgPosition == null
            {
                throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");
            }

            int? nextOrgPositionId = 0;

            int status = (int)StatusApplicationFormEnum.PENDING;

            var orgPosition = await _context.OrgPositions.FirstOrDefaultAsync(e => e.Id == request.OrgPositionId) ?? throw new ValidationException("Bạn chưa được cập nhật vị trí, liên hệ với HR");

            var approvalFlowCurrentPositionId = await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.RequestTypeId == (int)RequestTypeEnum.FORM_IT && e.FromOrgPositionId == orgPosition.Id);

            //GM
            if (approvalFlowCurrentPositionId != null)
            {
                nextOrgPositionId = approvalFlowCurrentPositionId.ToOrgPositionId; //manager IT
                status = (int)StatusApplicationFormEnum.FINAL_APPROVAL;
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
                UserCodeCreated = request.UserCodeCreated,
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

            _context.ITFormCategories.AddRange(itFormCategories);

            await _context.SaveChangesAsync();

            var nextUserInfo = await _userService.GetMultipleUserViclockByOrgPositionId(nextOrgPositionId ?? -1);

            //send email

            //add new application form
            //save to db
            //find next user
            //send email

            return true;
        }

        public Task<object> Delete(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAll(GetAllITFormRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<object> Update(Guid Id, UpdateITFormRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
