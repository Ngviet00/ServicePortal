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

            var query = _context.LeaveRequests.AsQueryable();


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
            var entity = LeaveRequestMapper.ToEntity(dto);

            entity.UpdatedAt = null;
            entity.DeletedAt = null;
            entity.CreatedAt = DateTime.Now;
            entity.Status = (byte?)StatusLeaveRequestEnum.PENDING;

            _context.LeaveRequests.Add(entity);

            await _context.SaveChangesAsync();

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
    }
}
