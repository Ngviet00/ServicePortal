using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.ApprovalFlow;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.ApprovalFlow
{
    public class ApprovalFlowService : IApprovalFlowService
    {
        private readonly ApplicationDbContext _context;

        public ApprovalFlowService (ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy luồng duyệt theo vị trí của người dùng và loại yêu cầu
        /// </summary>
        public async Task<Domain.Entities.ApprovalFlow?> GetWorkFlowByFromOrgUnitIdAndRequestType(int? fromOrgPositionid, int? requestTypeId)
        {
            return await _context.ApprovalFlows.FirstOrDefaultAsync(e => e.FromOrgPositionId == fromOrgPositionid && e.RequestTypeId == requestTypeId);
        }
    }
}
