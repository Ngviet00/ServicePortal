using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServicePortals.Application.Interfaces.Approval;

namespace ServicePortals.Application.Services.Approval
{
    public class ApprovalService : IApprovalService
    {
        public Task<object> Approval()
        {
            throw new NotImplementedException();
        }

        public Task<object> ListAssigned()
        {
            throw new NotImplementedException();
        }

        public Task<object> ListHistoryApprovedOrProcessed()
        {
            throw new NotImplementedException();
        }

        public Task<object> ListWaitApproval()
        {
            throw new NotImplementedException();
        }
    }
}
