using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.OverTime.Requests;
using ServicePortals.Application.Dtos.OverTime.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Interfaces.OverTime
{
    public interface IOverTimeService
    {
        /// <summary>
        /// Lấy tất cả loại tăng ca, tăng ca ngày thường, chủ nhật , ngày lễ
        /// </summary>
        Task<List<TypeOverTime>> GetAllTypeOverTime(); //done

        /// <summary>
        /// Tạo đơn tăng ca
        /// </summary>
        Task<object> Create(CreateOverTimeRequest request); //done

        /// <summary>
        /// Update overtime
        /// </summary>
        Task<object> Update(string applicationFormCode, CreateOverTimeRequest request); //done

        /// <summary>
        /// Hiển thị tăng ca của cá nhân
        /// </summary>
        Task<PagedResults<MyOverTimeResponse>> GetMyOverTime(MyOverTimeRequest request); //done

        /// <summary>
        /// Hiển thị các đơn đã đăng ký tăng ca, đăng ký cho cá nhân - đky cho người khác
        /// </summary>
        Task<PagedResults<MyOverTimeRegisterResponse>> GetOverTimeRegister(MyOverTimeRequest request); //done

        /// <summary>
        /// Xóa đơn tăng ca dựa vào mã đơn
        /// </summary>
        Task<object> Delete(string applicationFormCode);  //done

        /// <summary>
        /// Từ chối 1 vài người tăng ca
        /// </summary>
        Task<object> RejectSomeOverTimes(RejectSomeOverTimeRequest request); //done

        /// <summary>
        /// Hr chú thích overtime của cá nhân nào đó
        /// </summary>
        Task<object> HrNote(HrNoteOverTimeRequest request); //done

        /// <summary>
        /// Chức năng duyệt
        /// </summary>
        Task<object> Approval(ApprovalRequest request);

        /// <summary>
        /// Lấy chi tiết đơn tăng ca, bao gồm cả thông tin đơn, trạng thái đơn,..
        /// </summary>
        Task<object> GetDetailOverTimeByApplicationFormCode(string applicationFormCode); //done

        /// <summary>
        /// HR export excel
        /// </summary>
        Task<byte[]> HrExportExcel(long applicationFormId); //done
    }
}
