using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.CustomApprovalFlow.DTO;
using ServicePortal.Modules.CustomApprovalFlow.Services.Interfaces;

namespace ServicePortal.Modules.CustomApprovalFlow.Services
{
    public class CustomApprovalFlowService : ICustomApprovalFlowService
    {
        //private readonly ApplicationDbContext _context;

        //public CustomApprovalFlowService(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task<PagedResults<CustomApprovalFlowDto>> GetAll(CustomApprovalFlowDto request)
        //{
        //    int? departmentId = request.DepartmentId;

        //    double pageSize = request.PageSize;

        //    double page = request.Page;

        //    var query = _context.CustomApprovalFlows.AsQueryable();

        //    if (departmentId != null)
        //    {
        //        query = query.Where(e => e.DepartmentId == departmentId);
        //    }

        //    var totalItems = await query.CountAsync();

        //    var totalPages = (int)Math.Ceiling(totalItems / pageSize);

        //    var customApprovalFlows = await query
        //        .Include(e => e.Department)
        //        .Skip((int)((page - 1) * pageSize))
        //        .Take((int)pageSize)
        //        .ToListAsync();

        //    var result = new PagedResults<CustomApprovalFlowDto>
        //    {
        //        Data = CustomApprovalFlowMapper.ToDtoList(customApprovalFlows),
        //        TotalItems = totalItems,
        //        TotalPages = totalPages
        //    };

        //    return result;
        //}

        //public async Task<CustomApprovalFlowDto> GetById(int id)
        //{
        //    var result = await _context.CustomApprovalFlows.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Data not found!");

        //    return CustomApprovalFlowMapper.ToDto(result);
        //}

        //public async Task<Domain.Entities.ApprovalFlow> Create(CustomApprovalFlowDto dto)
        //{
        //    var entity = new Domain.Entities.ApprovalFlow
        //    {
        //        DepartmentId = dto.DepartmentId ?? null,
        //        TypeCustomApproval = dto.TypeCustomApproval ?? null,
        //        From = dto.From ?? null,
        //        To = dto.To ?? null
        //    };

        //    _context.CustomApprovalFlows.Add(entity);
        //    await _context.SaveChangesAsync();

        //    return entity;
        //}

        //public async Task<Domain.Entities.ApprovalFlow> Update(int id, CustomApprovalFlowDto dto)
        //{
        //    var entity = await _context.CustomApprovalFlows.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Data not found!");

        //    entity.DepartmentId = dto.DepartmentId ?? null;
        //    entity.TypeCustomApproval = dto.TypeCustomApproval ?? null;
        //    entity.From = dto.From ?? null;
        //    entity.To = dto.To ?? null;

        //    _context.CustomApprovalFlows.Update(entity);
        //    await _context.SaveChangesAsync();

        //    return entity;
        //}

        //public async Task<Domain.Entities.ApprovalFlow> Delete(int id)
        //{
        //    var result = await _context.CustomApprovalFlows.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Data not found!");

        //    _context.CustomApprovalFlows.Remove(result);

        //    await _context.SaveChangesAsync();

        //    return result;
        //}
    }
}
