using ServicePortals.Application.Dtos.LeaveRequest;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Infrastructure.Mappers
{
    public class LeaveRequestMapper
    {
        public static LeaveRequestDto ToDto(LeaveRequest entity, ApplicationForm? applicationForm = null, HistoryApplicationForm? historyApplicationForm = null)
        {
            return new LeaveRequestDto
            {
                Id = entity.Id,
                RequesterUserCode = entity.RequesterUserCode,
                UserNameWriteLeaveRequest = entity.UserNameWriteLeaveRequest,
                Name = entity.Name,
                FromDate = entity.FromDate,
                ToDate = entity.ToDate,
                TypeLeave = entity?.TypeLeave,
                TimeLeave = entity?.TimeLeave,
                Reason = entity?.Reason,
                Department = entity?.Department,
                Position = entity?.Position,
                HaveSalary = entity?.HaveSalary,
                Image = null,
                CreatedAt = entity?.CreatedAt,
                ApplicationForm = applicationForm,
                HistoryApplicationForm = historyApplicationForm
            };
        }

        public static List<LeaveRequestDto> ToDtoList(List<(LeaveRequest LeaveRequest, ApplicationForm? ApplicationForm, HistoryApplicationForm? HistoryApplicationForm)> list)
        {
            return list.Select(x => ToDto(x.LeaveRequest, x.ApplicationForm, x.HistoryApplicationForm)).ToList();
        }
    }
}
