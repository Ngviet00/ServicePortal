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
        //    private readonly ApplicationDbContext _context;

        //    public LeaveRequestService(ApplicationDbContext context)
        //    {
        //        _context = context;
        //    }

        //    public async Task<PagedResults<LeaveRequestDTO>> GetAll(GetAllLeaveRequest request)
        //    {
        //        double pageSize = request.PageSize;

        //        double page = request.Page;

        //        byte? status = request.Status;

        //        var query = _context.LeaveRequests.AsQueryable();

        //        //find leave request of current user
        //        query = query.Where(e => e.UserCode == request.UserCode);

        //        //default get status leave request is pending
        //        query = query.Where(e => e.Status == (status > 1 ? status : 1));

        //        var totalItems = await query.CountAsync();

        //        var totalPages = (int)Math.Ceiling(totalItems / pageSize);

        //        var leaveRequests = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize).ToListAsync();

        //        return new PagedResults<LeaveRequestDTO>
        //        {
        //            Data = LeaveRequestMapper.ToDtoList(leaveRequests),
        //            TotalItems = totalItems,
        //            TotalPages = totalPages
        //        };
        //    }

        //    public async Task<LeaveRequestDTO> GetById(Guid id)
        //    {
        //        var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

        //        return LeaveRequestMapper.ToDto(leaveRequest);
        //    }

        //    public async Task<LeaveRequestDTO> Create(LeaveRequestDTO dto)
        //    {
        //        var entity = LeaveRequestMapper.ToEntity(dto);

        //        entity.UpdatedAt = null;
        //        entity.DeletedAt = null;
        //        entity.CreatedAt = DateTime.Now;
        //        entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;

        //        _context.LeaveRequests.Add(entity);

        //        await _context.SaveChangesAsync();

        //        //get user approval => meaning position level next
        //        var userHigher = await FindUserWithHigherPosition(entity.UserCode ?? "");

        //        Domain.Entities.LeaveRequestStep leaveRequestStep = new()
        //        {
        //            LeaveRequestId = entity.Id,
        //            PositionIdApproval = userHigher.Position.Id,
        //            StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING,
        //        };

        //        _context.LeaveRequestSteps.Add(leaveRequestStep);
        //        await _context.SaveChangesAsync();


        //        return dto;
        //    }

        //    public async Task<LeaveRequestDTO> Update(Guid id, LeaveRequestDTO dto)
        //    {
        //        var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

        //        leaveRequest = LeaveRequestMapper.ToEntity(dto);
        //        leaveRequest.Id = id;

        //        _context.LeaveRequests.Update(leaveRequest);

        //        await _context.SaveChangesAsync();

        //        return LeaveRequestMapper.ToDto(leaveRequest);
        //    }

        //    public async Task<LeaveRequestDTO> Delete(Guid id)
        //    {
        //        var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Leave request not found!");

        //        _context.LeaveRequests.Remove(leaveRequest);

        //        await _context.SaveChangesAsync();

        //        return LeaveRequestMapper.ToDto(leaveRequest);
        //    }

        //    public async Task<Domain.Entities.User?> FindUserWithHigherPosition(string userCodeWriteLeaveRequest)
        //    {
        //        var currentUser = await _context.Users.Include(e => e.Position).FirstOrDefaultAsync(u => u.Code == userCodeWriteLeaveRequest);

        //        if (currentUser == null || currentUser.Position == null)
        //        {
        //            return null;
        //        }

        //        var currentLevel = currentUser.Position.Level;
        //        var parentDeptId = currentUser.ParentDepartmentId;
        //        var childDeptId = currentUser.ChildDepartmentId;

        //        var userHightPosition = await _context.Users
        //            .Include(u => u.Position)
        //            .Where(u =>
        //                u.Position.Level == currentLevel - 1 &&
        //                (u.ParentDepartmentId == parentDeptId || u.ChildDepartmentId == childDeptId))
        //            .Select(u => new Domain.Entities.User {
        //                Id = u.Id,
        //                Name = u.Name,
        //                Position = u.Position
        //            })
        //            .FirstOrDefaultAsync();

        //        if (userHightPosition != null)
        //        {
        //            return userHightPosition;
        //        }

        //        return null;
        //    }

        //    public async Task<PagedResults<LeaveRequestDTO>> GetAllWaitApproval(GetAllLeaveRequest request)
        //    {
        //        double pageSize = request.PageSize;

        //        double page = request.Page;

        //        int? positionId = request.PositionId;

        //        var query = _context.LeaveRequests
        //            .Join(
        //                _context.LeaveRequestSteps,
        //                lr => lr.Id,
        //                lrs => lrs.LeaveRequestId,
        //                (lr, lrs) => new { LeaveRequest = lr, LeaveRequestStep = lrs }
        //            )
        //            .Where(x => x.LeaveRequestStep.PositionIdApproval == positionId && x.LeaveRequestStep.StatusStep == (byte)StatusLeaveRequestStepEnum.PENDING);

        //        var totalItems = await query.CountAsync();

        //        var totalPages = (int)Math.Ceiling(totalItems / pageSize);

        //        var leaveRequests = await query.Skip((int)((page - 1) * pageSize)).Take((int)pageSize)
        //            .Select(x => x.LeaveRequest)
        //            .ToListAsync();

        //        return new PagedResults<LeaveRequestDTO>
        //        {
        //            Data = LeaveRequestMapper.ToDtoList(leaveRequests),
        //            TotalItems = totalItems,
        //            TotalPages = totalPages
        //        };
        //    }

        //    public async Task<LeaveRequestDTO?> Approval(ApprovalDTO request)
        //    {
        //        var user = await _context.Users.Include(e => e.Position).FirstOrDefaultAsync(e => e.Code == request.UserCodeApproval);

        //        if (user == null)
        //        {
        //            throw new NotFoundException("User not found!");
        //        }


        //        var leaveRequestStep = await _context.LeaveRequestSteps.Where(e => e.LeaveRequestId == Guid.Parse(request.LeaveRequestId) && e.PositionIdApproval == user.PositionId).FirstOrDefaultAsync();

        //        if (leaveRequestStep == null)
        //        {
        //            return null;
        //        }

        //        leaveRequestStep.Id = leaveRequestStep.Id;
        //        leaveRequestStep.ApprovedBy = user.Name;
        //        leaveRequestStep.CodeApprover = user.Code;
        //        leaveRequestStep.StatusStep = request.Status ? (byte)StatusLeaveRequestStepEnum.APPROVAL : (byte)StatusLeaveRequestStepEnum.REJECT;
        //        leaveRequestStep.ApprovedAt = DateTime.Now;

        //        _context.LeaveRequestSteps.Update(leaveRequestStep);
        //        await _context.SaveChangesAsync();

        //        //fake send to hr and wait hr approval
        //        if (request.Status)
        //        {
        //            Domain.Entities.LeaveRequestStep leaveRequestStepNew = new Domain.Entities.LeaveRequestStep
        //            {
        //                LeaveRequestId = Guid.Parse(request.LeaveRequestId),
        //                PositionIdApproval = 99, //position hr am
        //                StatusStep = (byte)StatusLeaveRequestStepEnum.PENDING
        //            };
        //        }

        //        var leaveRequest = await _context.LeaveRequests.FirstOrDefaultAsync(e => e.Id == Guid.Parse(request.LeaveRequestId));

        //        if (leaveRequest == null)
        //        {
        //            return null;
        //        }

        //        if (request.Status)
        //        {
        //            leaveRequest.Status = (byte)StatusLeaveRequestEnum.IN_PROCESS;
        //        }
        //        else
        //        {
        //            leaveRequest.Status = (byte)StatusLeaveRequestEnum.REJECT;
        //        }

        //        _context.LeaveRequests.Update(leaveRequest);
        //        await _context.SaveChangesAsync();

        //        return LeaveRequestMapper.ToDto(leaveRequest);
        //    }
    }
}
