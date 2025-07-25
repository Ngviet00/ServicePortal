using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Interfaces.WorkFlowStep;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.WorkFlowStep
{
    public class WorkFlowStepService : IWorkFlowStepService
    {
        private readonly ApplicationDbContext _context;

        public WorkFlowStepService (ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy luồng duyệt theo vị trí của người dùng và loại yêu cầu
        /// </summary>
        public async Task<Domain.Entities.WorkFlowStep?> GetWorkFlowByFromOrgUnitIdAndRequestType(int? fromOrgUnitId, int? requestTypeId)
        {
            return await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.FromOrgUnitId == fromOrgUnitId && e.RequestTypeId == requestTypeId);
        }
    }
}
