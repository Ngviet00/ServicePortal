using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.ITForm.Requests;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Application.Services.ITForm
{
    public class ITFormServiceImpl : Interfaces.ITForm.ITFormService
    {
        private readonly ApplicationDbContext _context;
        public ITFormServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<object> Approval(ApprovalRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> Create(CreateITFormRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> Delete(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetAll(GetAllITFormRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetById(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<object> Update(Guid Id, UpdateITFormRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
