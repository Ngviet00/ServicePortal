using Hangfire;
using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Domain.Enums;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Infrastructure.Email;
using ServicePortal.Modules.LeaveRequest.DTO;
using ServicePortal.Modules.LeaveRequest.DTO.Requests;
using ServicePortal.Modules.LeaveRequest.Services.Interfaces;

namespace ServicePortal.Modules.LeaveRequest.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ApplicationDbContext _context;

        public LeaveRequestService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAll(GetAllLeaveRequestDto request)
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
                    ApprovedBy = lr.LeaveRequestSteps != null ? lr.LeaveRequestSteps
                        .Where(s => s.ApprovedBy != null && s.ApprovedAt != null)
                        .OrderByDescending(s => s.ApprovedAt)
                        .Select(s => s.ApprovedBy)
                        .FirstOrDefault() : null
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

            return new PagedResults<LeaveRequestDto>
            {
                Data = leaveRequestDTOs,
                TotalItems = totalItems,
                TotalPages = totalPages,
                CountPending = countPending,
                CountInProcess = countInProcess,
            };
        }

        public async Task<LeaveRequestDto> GetById(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        //can check more have role send yourselt to hr
        public async Task<LeaveRequestDto> Create(LeaveRequestDto dto)
        {
            //get current user send leave request
            var user = await _context.Users.Include(e => e.Department).Where(e => e.Code == dto.UserCodeRegister).FirstOrDefaultAsync();

            //check have custom 
            var haveCustomApproval = await _context
                .CustomApprovalFlows
                .FirstOrDefaultAsync(e => e.TypeCustomApproval == "LEAVE_REQUEST" && e.DepartmentId == user!.DepartmentId && e.From == user.Level);


            Domain.Entities.User? nextUserCustomApproval = null;

            bool flagHaveCustomApproval = false;

            if (haveCustomApproval != null)
            {
                nextUserCustomApproval = await _context.Users.Where(e => e.DepartmentId == user!.DepartmentId && e.Level == haveCustomApproval.To).FirstOrDefaultAsync();
                flagHaveCustomApproval = true;
            }

            var nextUserApproval = await _context.Users.Where(e => e.DepartmentId == user!.DepartmentId && e.Level == user.LevelParent).FirstOrDefaultAsync();

            //create leave request
            var entity = LeaveRequestMapper.ToEntity(dto);

            entity.UpdatedAt = null;
            entity.DeletedAt = null;
            entity.CreatedAt = DateTime.Now;
            entity.DepartmentId = user?.Department?.Id ?? null;

            entity.Status = (byte?)StatusLeaveRequestEnum.IN_PROCESS;

            Domain.Entities.LeaveRequestStep lrStepData = new Domain.Entities.LeaveRequestStep
            {
                StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
            };

            var email = string.Empty;
            lrStepData.LevelApproval = "HR";
            email = "nguyenviet@vsvn.com.vn"; //email hr

            if ((nextUserApproval != null && flagHaveCustomApproval == false) || (flagHaveCustomApproval && nextUserCustomApproval == null))
            {
                lrStepData.LevelApproval = nextUserApproval!.Level;
                email = nextUserApproval.Email;

                entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;
            }
            else if (flagHaveCustomApproval && nextUserCustomApproval != null)
            {
                lrStepData.LevelApproval = nextUserCustomApproval.Level;
                email = nextUserCustomApproval.Email;

                entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;
            }

            _context.LeaveRequests.Add(entity);

            lrStepData.LeaveRequestId = entity.Id;

            _context.LeaveRequestSteps.Add(lrStepData);

            //send email
            if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
            {
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { email ?? "" }, entity, dto.UrlFrontEnd));
            }

            await _context.SaveChangesAsync();

            return dto;
        }

        public async Task<LeaveRequestDto> Update(Guid id, LeaveRequestDto dto)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            var updateEntity = LeaveRequestMapper.ToEntity(dto);

            updateEntity.Id = id;
            updateEntity.Status = (byte)StatusLeaveRequestEnum.PENDING;
            updateEntity.DepartmentId = leaveRequest.DepartmentId;
            updateEntity.CreatedAt = leaveRequest.CreatedAt;
            updateEntity.UpdatedAt = DateTime.Now;

            _context.LeaveRequests.Update(updateEntity);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<LeaveRequestDto> Delete(Guid id)
        {
            var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

            _context.LeaveRequests.Remove(leaveRequest);

            await _context.SaveChangesAsync();

            return LeaveRequestMapper.ToDto(leaveRequest);
        }

        public async Task<PagedResults<LeaveRequestDto>> GetAllWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
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
                    Roles = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role!.Name).ToList(),
                    IsRoleHR = u.UserRoles
                    .Any(ur => ur.Role != null && ur.Role!.Code == "HR")
                }).FirstOrDefaultAsync();

            if (user == null)
            {
                return new PagedResults<LeaveRequestDto>
                {
                    Data = [],
                    TotalItems = 0,
                    TotalPages = 0
                };
            }

            IQueryable<Domain.Entities.LeaveRequest> query;

            //get list approval by hr
            if (user.IsRoleHR)
            {
                query = QueryWaitApprovalHR(user?.Department?.Id, user?.Level);
            }
            else //get list approval user normal
            {
                query = QueryWaitApprovalNormalUser(request?.DepartmentId, request?.Level);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            //continues get more data name approval
            var leaveRequests = await query
                .Select(lr => new
                {
                    LeaveRequest = lr,
                    ApprovedBy = lr.LeaveRequestSteps != null ? lr.LeaveRequestSteps
                        .Where(s => s.ApprovedBy != null && s.ApprovedAt != null)
                        .OrderByDescending(s => s.ApprovedAt)
                        .Select(s => s.ApprovedBy)
                        .FirstOrDefault() : null
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

            return new PagedResults<LeaveRequestDto>
            {
                Data = leaveRequestDTOs,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<LeaveRequestDto?> Approval(ApprovalDto request, string currentUserCodeInJwt)
        {
            //check current user is user in jwt token
            if (string.IsNullOrWhiteSpace(currentUserCodeInJwt))
            {
                throw new ForbiddenException("User forbidden!");
            }

            if (currentUserCodeInJwt.Trim() != request.UserCodeApproval)
            {
                throw new ForbiddenException("User forbidden!");
            }

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
                    UserPermission = u.UserPermission
                        .Where(up => up.Permission != null)
                        .Select(up => up.Permission!.Name).ToList(),
                    Roles = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role!.Code).ToList(),
                    IsRoleHR = u.UserRoles
                        .Any(ur => ur.Role != null && ur.Role.Code == "HR"),
                    IsRoleHRManager = u.UserRoles
                        .Any(ur => ur.Role != null && ur.Role.Code == "HR_Manager")
                })
                .FirstOrDefaultAsync(e => e.Code == request.UserCodeApproval);

            if (userApproval == null)
            {
                throw new NotFoundException("Not found user approval");
            }

            //check have custom 
            var haveCustomApproval = await _context
                .CustomApprovalFlows
                .FirstOrDefaultAsync(e => e.TypeCustomApproval == "LEAVE_REQUEST" && e.DepartmentId == userApproval.DepartmentId && e.From == userApproval.Level);


            Domain.Entities.User? nextUserCustomApproval = null;

            bool flagHaveCustomApproval = false;

            if (haveCustomApproval != null)
            {
                nextUserCustomApproval = await _context.Users.Where(e => e.DepartmentId == userApproval.DepartmentId && e.Level == haveCustomApproval.To).FirstOrDefaultAsync();
                flagHaveCustomApproval = true;
            }

            var leaveRequest = await _context
                .LeaveRequests
                .FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId ?? ""));

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

            //------- case reject

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

            //------- case approval

            //case hr approval for other member
            if ((userApproval.IsRoleHR && leaveRequestStep.LevelApproval == "HR") || userApproval.IsRoleHRManager)
            {
                if (!userApproval.IsRoleHRManager)
                {
                    if (!userApproval.Roles.Any(r => r != null && r.Contains("leave_request.hr_approval")))
                    {
                        throw new ForbiddenException("Bạn chưa có quyền, liên hệ team IT");
                    }
                }

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
                        BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { nextUserApproval.Email }, leaveRequest, request.UrlFrontEnd));
                    }
                }

                await _context.SaveChangesAsync();

                return LeaveRequestMapper.ToDto(leaveRequest);
            }

            if (userApproval.Roles.Any(role => role != null && role.Contains("leave_request.approval_to_hr")))
            {
                Domain.Entities.LeaveRequestStep newStep = new Domain.Entities.LeaveRequestStep
                {
                    LeaveRequestId = leaveRequest.Id,
                    LevelApproval = "HR",
                    StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
                };
                _context.LeaveRequestSteps.Add(newStep);

                //send email to group hr, now fake 
                BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { "nguyenviet@vsvn.com.vn" }, leaveRequest, request.UrlFrontEnd));
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
                };

                newStep.LevelApproval = "HR";
                var email = "nguyenviet@vsvn.com.vn"; //email hr

                if ((nextUserApproval != null && flagHaveCustomApproval == false) || (flagHaveCustomApproval && nextUserCustomApproval == null))
                {
                    newStep.LevelApproval = nextUserApproval?.Level;
                    email = nextUserApproval?.Email;
                }
                else if (flagHaveCustomApproval && nextUserCustomApproval != null)
                {
                    newStep.LevelApproval = nextUserCustomApproval.Level;
                    email = nextUserCustomApproval.Email;
                }

                _context.LeaveRequestSteps.Add(newStep);

                if (!string.IsNullOrWhiteSpace(nextUserApproval?.Email))
                {
                    BackgroundJob.Enqueue<EmailService>(job => job.SendEmailLeaveRequest(new List<string> { email ?? "" }, leaveRequest, request.UrlFrontEnd));
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

        public async Task<int> CountWaitApproval(GetAllLeaveRequestWaitApprovalDto request)
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
                    Roles = u.UserRoles
                        .Where(ur => ur.Role != null)
                        .Select(ur => ur.Role!.Name).ToList(),
                    IsRoleHR = u.UserRoles
                        .Any(ur => ur.Role != null && ur.Role.Code!.ToUpper() == "HR")
                }).FirstOrDefaultAsync();

            if (user != null)
            {
                if (user.IsRoleHR)
                {
                    query = QueryWaitApprovalHR(user?.Department?.Id, user?.Level);
                }
                else
                {
                    query = QueryWaitApprovalNormalUser(request?.DepartmentId, request?.Level);
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
