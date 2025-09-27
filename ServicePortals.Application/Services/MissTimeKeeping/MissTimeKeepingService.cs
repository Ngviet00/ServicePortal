using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MissTimeKeeping.Requests;
using ServicePortals.Application.Dtos.MissTimeKeeping.Responses;
using ServicePortals.Application.Interfaces.MissTimeKeeping;

namespace ServicePortals.Application.Services.MissTimeKeeping;

public class MissTimeKeepingService : IMissTimeKeepingService
{
    public Task<object> Create(CreateMissTimeKeepingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<object> Update(string applicationFormCode, CreateMissTimeKeepingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResults<MyMissTimeKeepingResponse>> GetMyMissTimeKeeping(MyMissTimeKeepingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResults<MyMissTimeKeepingRegisterResponse>> GetMissTimeKeepingRegister(MyMissTimeKeepingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<object> Delete(string applicationFormCode)
    {
        throw new NotImplementedException();
    }

    public Task<object> HrNote(HrNoteMissTimeKeepingRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<object> Approval(ApprovalRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<object> GetMissTimeKeepingByApplicationFormCode(string applicationFormCode)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> HrExportExcel(long applicationFormId)
    {
        throw new NotImplementedException();
    }
}