using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
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

            var countPending = await query.Where(e => e.Status == (byte)StatusLeaveRequestEnum.PENDING).CountAsync();

            var countInProcess = await query.Where(e => e.Status == (byte)StatusLeaveRequestEnum.IN_PROCESS).CountAsync();

            //default get status leave request is pending
            query = query.Where(e => e.Status == (status > 1 ? status : 1));

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var leaveRequests = await query
                .Select(lr => new
                {
                    LeaveRequest = lr,
                    ApprovedBy = lr.LeaveRequestSteps
                        .Where(s => s.ApprovedBy != null && s.ApprovedAt != null)
                        .OrderByDescending(s => s.ApprovedAt)
                        .Select(s => s.ApprovedBy)
                        .FirstOrDefault()
                })
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var leaveRequestDTOs = leaveRequests.Select(x =>
            {
                var leaveRequestDto = LeaveRequestMapper.ToDto(x.LeaveRequest);
                leaveRequestDto.ApprovedBy = x.ApprovedBy;
                return leaveRequestDto;
            }).ToList();

            return new PagedResults<LeaveRequestDTO>
            {
                Data = leaveRequestDTOs,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CountPending = countPending,
                CountInProcess = countInProcess,
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

            //get next user
            var nextUserApproval = await _context.Users.Where(e => e.DepartmentId == user.DepartmentId && e.Level == user.LevelParent).FirstOrDefaultAsync();

            //create leave request
            var entity = LeaveRequestMapper.ToEntity(dto);

            entity.UpdatedAt = null;
            entity.DeletedAt = null;
            entity.CreatedAt = DateTime.Now;
            entity.DepartmentId = user?.Department?.Id ?? null;
            entity.Status = nextUserApproval == null ? (byte?)StatusLeaveRequestEnum.IN_PROCESS :(byte?)StatusLeaveRequestEnum.PENDING;
            _context.LeaveRequests.Add(entity);

            Domain.Entities.LeaveRequestStep lrStepData = new Domain.Entities.LeaveRequestStep
            {
                LeaveRequestId = entity.Id,
                LevelApproval = nextUserApproval == null ? "HR" : user?.LevelParent ?? null,
                StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            };
            _context.LeaveRequestSteps.Add(lrStepData);
            
            //send mail
            if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(nextUserApproval.Email, entity));
            }

            await _context.SaveChangesAsync();

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
                .Where(e => e.DeletedAt == null && e.DepartmentId == departmentId && e.Level == level)
                .Include(e => e.Department)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Level,
                    u.LevelParent,
                    u.DepartmentId,
                    u.Department,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    IsRoleHR = u.UserRoles.Any(ur => ur.Role.Code == "HR")
                }).FirstOrDefaultAsync();

            IQueryable<Domain.Entities.LeaveRequest> query;

            //get list approval by hr
            if (user.IsRoleHR)
            {
                query = QueryWaitApprovalHR(user.Department.Id, user.Level);
            }
            else //get list approval user normal
            {
                query = QueryWaitApprovalNormalUser(request?.DepartmentId, request.Level);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            //continues get more data name approval
            var leaveRequests = await query
                .Select(lr => new
                {
                    LeaveRequest = lr,
                    ApprovedBy = lr.LeaveRequestSteps
                        .Where(s => s.ApprovedBy != null && s.ApprovedAt != null)
                        .OrderByDescending(s => s.ApprovedAt)
                        .Select(s => s.ApprovedBy)
                        .FirstOrDefault()
                })
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var leaveRequestDTOs = leaveRequests.Select(x =>
            {
                var leaveRequestDto = LeaveRequestMapper.ToDto(x.LeaveRequest);
                leaveRequestDto.ApprovedBy = x.ApprovedBy;
                return leaveRequestDto;
            }).ToList();

            return new PagedResults<LeaveRequestDTO>
            {
                Data = leaveRequestDTOs,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveRequestDTO?> Approval(ApprovalDTO request)
        {
            var userApproval = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Code,
                    u.Name,
                    u.Level,
                    u.LevelParent,
                    u.DepartmentId,
                    u.Department,
                    UserPermission = u.UserPermission.Select(up => up.Permission.Name).ToList(),
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    IsRoleHR = u.UserRoles.Any(ur => ur.Role.Code == "HR"),
                    IsRoleHRManager = u.UserRoles.Any(ur => ur.Role.Code == "HR_Manager")
                })
                .FirstOrDefaultAsync(e => e.Code == request.UserCodeApproval);

            if (userApproval == null)
            {
                throw new NotFoundException("Not found user approval");
            }

            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""));

            if (leaveRequest == null)
            {
                throw new NotFoundException("Leave request not found!");
            }

            var leaveRequestStep = await _context.LeaveRequestSteps
                .FirstOrDefaultAsync(e =>
                    e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING &&
                    e.LeaveRequestId == Guid.Parse(request.LeaveRequestId ?? "")
                );

            if (leaveRequestStep == null)
            {
                throw new NotFoundException("Leave request step not found!");
            }

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
            }

            //check case hr approval of user normal
            //now only check code is hr and level step is hr
            if ((userApproval.IsRoleHR && leaveRequestStep.LevelApproval == "HR") || userApproval.IsRoleHRManager)
            {
                leaveRequest.Status = (byte)StatusLeaveRequestEnum.COMPLETE;
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

            //case hr approval for hr
            if (userApproval.IsRoleHR)
            {
                var nextUserApproval = await GetNextUserApproval(userApproval.DepartmentId, userApproval.LevelParent);

                leaveRequest.Status = nextUserApproval == null ? (byte)StatusLeaveRequestEnum.COMPLETE : (byte)StatusLeaveRequestEnum.IN_PROCESS;
                leaveRequest.UpdatedAt = DateTime.Now;
                _context.LeaveRequests.Update(leaveRequest);

                leaveRequestStep.UserCodeApprover = userApproval.Code;
                leaveRequestStep.StatusStep = (byte)StatusLeaveRequestStepEnum.APPROVAL;
                leaveRequestStep.ApprovedBy = userApproval.Name;
                leaveRequestStep.ApprovedAt = DateTime.Now;
                _context.LeaveRequestSteps.Update(leaveRequestStep);

                if (nextUserApproval != null)
                {
                    Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                    {
                        LeaveRequestId = leaveRequest.Id,
                        StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                        LevelApproval = nextUserApproval?.Level ?? null
                    };

                    _context.LeaveRequestSteps.Add(newStep);

                    if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
                    {
                        BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(nextUserApproval.Email, leaveRequest));
                    }
                }

                await _context.SaveChangesAsync();

                return LeaveRequestMapper.ToDto(leaveRequest);
            }

            //fake meaning have permission approval send to hr
            if (userApproval.UserPermission.Contains("leave_request.can_send_to_hr"))
            {
                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = leaveRequest.Id,
                    LevelApproval = "HR",
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                };
                _context.LeaveRequestSteps.Add(newStep);

                //send email to group hr, now fake 
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest("nguyenviet@vsvn.com.vn", leaveRequest));
            }
            else
            {
                leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;

                var nextUserApproval = await _context.Users.FirstOrDefaultAsync(e =>
                    e.DepartmentId == userApproval.DepartmentId &&
                    e.Level == userApproval.LevelParent
                );

                //exception when not found user approval, default send to hr

                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = leaveRequest.Id,
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                    LevelApproval = nextUserApproval == null ? "HR" : nextUserApproval?.Level ?? null
                };

                _context.LeaveRequestSteps.Add(newStep);

                if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
                {
                    BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(nextUserApproval.Email, leaveRequest));
                }
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
                .Where(e => e.DeletedAt == null && e.DepartmentId == request.DepartmentId && e.Level == request.Level)
                .Include(e => e.Department)
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Level,
                    u.LevelParent,
                    u.DepartmentId,
                    u.Department,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    IsRoleHR = u.UserRoles.Any(ur => ur.Role.Code.ToUpper() == "HR")
                }).FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.IsRoleHR)
                {
                    query = QueryWaitApprovalHR(user.Department.Id, user.Level);
                }
                else
                {
                    query = QueryWaitApprovalNormalUser(request?.DepartmentId, request.Level);
                }

                return await query.CountAsync();
            }

            return 0;
        }

        public IQueryable<Domain.Entities.LeaveRequest> QueryWaitApprovalHR(int? departmentId, string? level)
        {
            var query = _context.LeaveRequests
                .Where(o => o.LeaveRequestSteps != null && 
                    (o.LeaveRequestSteps.Any(o => o.LevelApproval == "HR" && 
                     o.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING) ||
                ((o.DepartmentId == departmentId) && 
                    o.LeaveRequestSteps.Any(e => e.LevelApproval == level && e.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING))));

            return query;
        }

        public IQueryable<Domain.Entities.LeaveRequest> QueryWaitApprovalNormalUser(int? departmentId, string? level)
        {
            var query = _context.LeaveRequests.Where(e => 
                    e.DepartmentId == departmentId && 
                    e.LeaveRequestSteps != null &&
                    e.LeaveRequestSteps.Any(x =>
                        x.LevelApproval == level &&
                        x.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING
                    )
                );

            return query;
        }

        public async Task<Domain.Entities.User?> GetNextUserApproval(int? departmentId, string? level)
        {
            var nextUserApproval = await _context.Users
                .FirstOrDefaultAsync(e =>
                    e.DepartmentId == departmentId &&
                    e.Level == level
                );

            return nextUserApproval;
        }
    }
}
