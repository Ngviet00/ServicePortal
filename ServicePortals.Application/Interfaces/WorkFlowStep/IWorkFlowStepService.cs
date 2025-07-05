using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicePortals.Application.Interfaces.WorkFlowStep
{
    public interface IWorkFlowStepService
    {
        Task<Domain.Entities.WorkFlowStep?> GetWorkFlowByFromOrgUnitIdAndRequestType(int? fromOrgUnitId, int? requestTypeId);
    }
}
