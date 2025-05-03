using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Entities;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.Interfaces;
using ServicePortal.Modules.LeaveRequest.Requests;

namespace ServicePortal.Modules.LeaveRequest.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        private readonly IEmailService _emailService;

        public LeaveRequestService(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
            //get current user send leave request
            var user = await _context.Users.Include(e => e.Department).Where(e => e.Code == dto.UserCodeRegister).FirstOrDefaultAsync();

            //create leave request
            var entity = LeaveRequestMapper.ToEntity(dto);

            entity.UpdatedAt = null;
            entity.DeletedAt = null;
            entity.CreatedAt = DateTime.Now;
            entity.DepartmentId = user?.Department?.Id ?? null;
            entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;
            _context.LeaveRequests.Add(entity);

            //check user have permission send to hr, position is manager, am,...
            if (false)
            {

            }
            else
            {
                Domain.Entities.LeaveRequestStep lrStepData = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = entity.Id,
                    LevelApproval = user?.LevelParent ?? null,
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                };
                _context.LeaveRequestSteps.Add(lrStepData);
            }

            await _context.SaveChangesAsync();

            //get next user
            var nextUserApproval = await _context.Users.Where(e => e.DepartmentId == user.DepartmentId && e.Level == user.LevelParent).FirstOrDefaultAsync();

            //send mail
            if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(nextUserApproval.Email, entity));
            }

            return dto;
        }

        public async Task<LeaveRequestDTO> Update(Guid id, LeaveRequestDTO dto)
        {
            //var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            //leaveRequest = LeaveRequestMapper.ToEntity(dto);
            //leaveRequest.Id = id;

            //_context.LeaveRequests.Update(leaveRequest);

            //await _context.SaveChangesAsync();

            //return LeaveRequestMapper.ToDto(leaveRequest);
            return null;
        }

        public async Task<LeaveRequestDTO> Delete(Guid id)
        {
            //var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            //_context.LeaveRequests.Remove(leaveRequest);

            //await _context.SaveChangesAsync();

            //return LeaveRequestMapper.ToDto(leaveRequest);
            return null;
        }

        public async Task<PagedResults<LeaveRequestDTO>> GetAllWaitApproval(GetAllLeaveRequestWaitApproval request)
        {
            double pageSize = request.PageSize;

            double page = request.Page;

            string level = request.Level ?? "";

            int? departmentId = request.DepartmentId;

            //get current user
            var user = await _context.Users
                .Include(e => e.Role)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => 
                    e.DepartmentId == departmentId && 
                    e.Level == level
                );

            IQueryable<Domain.Entities.LeaveRequest> query;

            if (user?.Role?.Code?.ToUpper() == "HR")
            {
                query = _context.LeaveRequests
                    .Where(o => o.LeaveRequestSteps != null && (o.LeaveRequestSteps.Any(o => o.LevelApproval == "HR") || 
                    ((o.DepartmentId == user.Department.Id) && o.LeaveRequestSteps.Any(e => e.LevelApproval == user.Level))));
            }
            else
            {
                query = _context.LeaveRequests
                    .Where(o => 
                        o.LeaveRequestSteps != null && 
                        o.DepartmentId == request.DepartmentId && o.LeaveRequestSteps.Any(e =>
                            e.LevelApproval == level &&
                            e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING)
                    );
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var roles = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

            var leaveRequests = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

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

            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""))
                ?? throw new NotFoundException("Leave request not found!");

            var leaveRequestStep = await _context.LeaveRequestSteps
                .FirstOrDefaultAsync(e => 
                    e.LevelApproval == userApproval.Level && 
                    e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING && 
                    e.LeaveRequestId == Guid.Parse(request.LeaveRequestId ?? "")) 
                ?? throw new NotFoundException("Leave request step not found!");

            //case reject
            if (request.Status == false)
            {
                leaveRequest.Status = (byte)StatusLeaveRequestEnum.REJECT;
                leaveRequest.Note = request.Note;
                _context.LeaveRequests.Update(leaveRequest);

                leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.REJECT;
                leaveRequestStep.ApprovedBy = userApproval.Name;
                leaveRequestStep.UserCodeApprover = userApproval.Code;
                leaveRequestStep.ApprovedAt = DateTime.Now;
                _context.LeaveRequestSteps.Update(leaveRequestStep);

                await _context.SaveChangesAsync();

                return LeaveRequestMapper.ToDto(leaveRequest);
            };

            //fake meaning have permission approval, send to hr
            if (userApproval.Level == "2.1")
            {
                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = leaveRequest.Id,
                    LevelApproval = "HR",
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                };
                _context.LeaveRequestSteps.Add(newStep);
            }
            else
            {
                leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;

                var nextUserApproval = await _context.Users.FirstOrDefaultAsync(e =>
                    e.DepartmentId == userApproval.DepartmentId &&
                    e.Level == userApproval.LevelParent
                );

                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = leaveRequest.Id,
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                    LevelApproval = nextUserApproval?.Level ?? null
                };

                _context.LeaveRequestSteps.Add(newStep);
            }
            leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;
            leaveRequest.UpdatedAt = DateTime.Now;
            _context.LeaveRequests.Update(leaveRequest);

            leaveRequestStep.UserCodeApprover = userApproval.Code;
            leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
            leaveRequestStep.ApprovedBy = userApproval.Name;
            leaveRequestStep.ApprovedAt = DateTime.Now;

            _context.LeaveRequestSteps.Update(leaveRequestStep);
            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<int> CountWaitApproval(GetAllLeaveRequestWaitApproval request)
        {
            IQueryable<Domain.Entities.LeaveRequest> query;

            var user = await _context.Users
                .Include(e => e.Role)
                .Include(e => e.Department)
                        .FirstOrDefaultAsync(e =>
                    e.DepartmentId == request.DepartmentId &&
                    e.Level == request.Level
                );

            if (user?.Role?.Code?.ToUpper() == "HR")
            {
                query = _context.LeaveRequests
                    .Where(o => o.LeaveRequestSteps != null && (o.LeaveRequestSteps.Any(o => o.LevelApproval == "HR") ||
                    ((o.DepartmentId == user.Department.Id) && o.LeaveRequestSteps.Any(e => e.LevelApproval == user.Level))));
            }
            else
            {
                query = _context.LeaveRequests.Where(e =>
                        e.DepartmentId == request.DepartmentId &&
                        e.LeaveRequestSteps != null &&
                        e.LeaveRequestSteps.Any(x =>
                            x.LevelApproval == request.Level &&
                            x.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING
                        )
                    );
            }
                
            return await query.CountAsync();
        }
    }
}
