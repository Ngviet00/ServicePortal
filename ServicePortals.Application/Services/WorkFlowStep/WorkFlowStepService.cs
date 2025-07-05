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

        public async Task<Domain.Entities.WorkFlowStep?> GetWorkFlowByFromOrgUnitIdAndRequestType(int? fromOrgUnitId, int? requestTypeId)
        {
            return await _context.WorkFlowSteps.FirstOrDefaultAsync(e => e.FromOrgUnitId == fromOrgUnitId && e.RequestTypeId == requestTypeId);
        }
    }
}
