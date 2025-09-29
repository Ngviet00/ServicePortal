using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.MissTimeKeeping.Requests;
using ServicePortals.Application.Dtos.MissTimeKeeping.Responses;

namespace ServicePortals.Application.Interfaces.MissTimeKeeping;

public interface IMissTimeKeepingService
{
    Task<object> Create(CreateMissTimeKeepingRequest request); //done
    
    Task<object> Update(string applicationFormCode, CreateMissTimeKeepingRequest request); //done

    /// <summary>
    /// Hiển thị tăng ca của cá nhân
    /// </summary>
    Task<PagedResults<MyMissTimeKeepingResponse>> GetMyMissTimeKeeping(MyMissTimeKeepingRequest request); //done

    /// <summary>
    /// Hiển thị các đơn đã đăng ký tăng ca, đăng ký cho cá nhân - đky cho người khác
    /// </summary>
    Task<PagedResults<MyMissTimeKeepingRegisterResponse>> GetMissTimeKeepingRegister(MyMissTimeKeepingRequest request); //done

    /// <summary>
    /// Xóa đơn dựa vào mã đơn
    /// </summary>
    Task<object> Delete(string applicationFormCode); //done

    /// <summary>
    /// Hr chú thích của cá nhân
    /// </summary>
    Task<object> HrNote(HrNoteMissTimeKeepingRequest request);

    /// <summary>
    /// Duyệt đơn
    /// </summary>
    Task<object> Approval(ApprovalRequest request);

    /// <summary>
    /// Lấy chi tiết đơn, bao gồm cả thông tin đơn, trạng thái đơn,..
    /// </summary>
    Task<object> GetMissTimeKeepingByApplicationFormCode(string applicationFormCode); //done

    /// <summary>
    /// HR export excel
    /// </summary>
    Task<byte[]> HrExportExcel(long applicationFormId);
}