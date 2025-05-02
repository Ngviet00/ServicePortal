using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.Interfaces;
using ServicePortal.Modules.LeaveRequest.Requests;

namespace ServicePortal.Modules.LeaveRequest.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<LeaveRequestDTO>> GetAll(GetAllLeaveRequest request)
        {
            double pageSize = request.PageSize;

            double page = request.Page;

            byte? status = request.Status;

            var query = _context.LeaveRequests.AsQueryable();

            //find leave request of current user
            query = query.Where(e => e.UserCode == request.UserCode);

            //default get status leave request is pending
            query = query.Where(e => e.Status == (status > 1 ? status : 1));

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var leaveRequests = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            return new PagedResults<LeaveRequestDTO>
            {
                Data = LeaveRequestMapper.ToDtoList(leaveRequests),
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveRequestDTO> GetById(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDTO> Create(LeaveRequestDTO dto)
        {
            //create leave request
            var entity = LeaveRequestMapper.ToEntity(dto);

            entity.UpdatedAt = null;
            entity.DeletedAt = null;
            entity.CreatedAt = DateTime.Now;
            entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;

            _context.LeaveRequests.Add(entity);

            await _context.SaveChangesAsync();

            //get current user send leave request
            var user = await _context.Users.Include(e => e.Department).Where(e => e.Code == dto.UserCodeRegister).FirstOrDefaultAsync();

            //get level of next user approval
            var levelNextUser = user?.LevelParent;
            var currentDepartmentId = user?.Department?.Id;

            //check has user with next level, if ok => send, else find next level
            var nextUserApproval = await _context.Users.Where(e => e.DepartmentId == currentDepartmentId && e.Level == levelNextUser).FirstOrDefaultAsync();

            //current then check exist user, if not find next level
            if (true)
            {

            }

            //add leave request step
            Domain.Entities.LeaveRequestStep lrStepData = new Domain.Entities.LeaveRequestStep
            {
                LeaveRequestId = entity.Id,
                UserCodeApprover = nextUserApproval?.Code,
                StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            };

            _context.LeaveRequestSteps.Add(lrStepData);
            await _context.SaveChangesAsync();

            //send mail

            return dto;
        }

        public async Task<LeaveRequestDTO> Update(Guid id, LeaveRequestDTO dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            leaveRequest = LeaveRequestMapper.ToEntity(dto);
            leaveRequest.Id = id;

            _context.LeaveRequests.Update(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDTO> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            _context.LeaveRequests.Remove(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<Domain.Entities.User?> FindUserWithHigherPosition(string userCodeWriteLeaveRequest)
        {
            //var currentUser = await _context.Users.Include(e => e.Position).FirstOrDefaultAsync(u => u.Code == userCodeWriteLeaveRequest);

            //if (currentUser == null || currentUser.Position == null)
            //{
            //    return null;
            //}

            //var currentLevel = currentUser.Position.Level;
            //var parentDeptId = currentUser.ParentDepartmentId;
            //var childDeptId = currentUser.ChildDepartmentId;

            //var userHightPosition = await _context.Users
            //    .Include(u => u.Position)
            //    .Where(u =>
            //        u.Position.Level == currentLevel - 1 &&
            //        (u.ParentDepartmentId == parentDeptId || u.ChildDepartmentId == childDeptId))
            //    .Select(u => new Domain.Entities.User
            //    {
            //        Id = u.Id,
            //        Name = u.Name,
            //        Position = u.Position
            //    })
            //    .FirstOrDefaultAsync();

            //if (userHightPosition != null)
            //{
            //    return userHightPosition;
            //}

            return null;
        }

        public async Task<PagedResults<LeaveRequestDTO>> GetAllWaitApproval(GetAllLeaveRequest request)
        {
            double pageSize = request.PageSize;

            double page = request.Page;

            string userCode = request?.UserCode ?? "";

            var query = _context.LeaveRequests
                .Join(
                    _context.LeaveRequestSteps,
                    lr => lr.Id,
                    lrs => lrs.LeaveRequestId,
                    (lr, lrs) => new { LeaveRequest = lr, LeaveRequestStep = lrs }
                )
                .Where(e => e.LeaveRequestStep.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING && e.LeaveRequestStep.UserCodeApprover == userCode);

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var roles = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var leaveRequests = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize)
                .Select(x => x.LeaveRequest)
                .ToListAsync();

            return new PagedResults<LeaveRequestDTO>
            {
                Data = LeaveRequestMapper.ToDtoList(leaveRequests),
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveRequestDTO?> Approval(ApprovalDTO request)
        {
            var userApproval = await _context.Users.FirstOrDefaultAsync(e => e.Code == request.UserCodeApproval);

            if (userApproval == null)
            {
                throw new NotFoundException("Not found user approval");
            }

            var itemLeaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? "")) ?? throw new NotFoundException("Leave request not found!");
            itemLeaveRequest.UpdatedAt = DateTime.Now;

            var itemLeaveRequestStep = await _context.LeaveRequestSteps.FirstOrDefaultAsync(e => e.UserCodeApprover == request.UserCodeApproval && e.LeaveRequestId == Guid.Parse(request.LeaveRequestId ?? "")) ?? throw new NotFoundException("Leave request step not found!");
            itemLeaveRequestStep.ApprovedBy = userApproval.Name;
            itemLeaveRequestStep.ApprovedAt = DateTime.Now;

            //check is hr, have permission or not, set status leave request step, leave request to complete or reject

            //case reject
            if (request.Status == false)
            {
                //update leave request with status reject
                itemLeaveRequest.Status = (byte)StatusLeaveRequestEnum.REJECT;
                itemLeaveRequest.Note = request.Note;
                _context.LeaveRequests.Update(itemLeaveRequest);

                //update leave request step with status reject                
                itemLeaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.REJECT;
                _context.LeaveRequestSteps.Update(itemLeaveRequestStep);

                await _context.SaveChangesAsync();

                return LeaveRequestMapper.ToDto(itemLeaveRequest);
            }

            //case approval
            itemLeaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;
            _context.LeaveRequests.Update(itemLeaveRequest);

            itemLeaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
            _context.LeaveRequestSteps.Update(itemLeaveRequestStep);


            //check have permission send to hr, check case approval is hr
            if (userApproval.Level == "2.1")
            {
                //send to hr
                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = itemLeaveRequest.Id,
                    UserCodeApprover = "HR_NTH",
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING
                };
                _context.LeaveRequestSteps.Add(newStep);
            }
            else
            {
                //find next user
                var nextUserApproval = await _context.Users.FirstOrDefaultAsync(e => e.Level == userApproval.LevelParent && e.DepartmentId == userApproval.DepartmentId);

                if (userApproval == null)
                {
                    throw new NotFoundException("Not found user approval");
                }

                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = itemLeaveRequest.Id,
                    UserCodeApprover = nextUserApproval?.Code ?? "",
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING
                };
                _context.LeaveRequestSteps.Add(newStep);
            }

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(itemLeaveRequest);
        }

        public async Task<int> CountWaitApproval(string userCode)
        {
            var result = await _context.LeaveRequestSteps.Where(e => e.UserCodeApprover == userCode && e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING).CountAsync();

            return result;
        }
    }
}
