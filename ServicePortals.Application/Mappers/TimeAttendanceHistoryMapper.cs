using ServicePortals.Application.Dtos.TimeKeeping;
using ServicePortals.Domain.Entities;
namespace ServicePortals.Application.Mappers
{
    public class TimeAttendanceHistoryMapper
    {
        public static TimeAttendanceHistoryDto ToDto(TimeAttendanceEditHistory entity)
        {
            if (entity == null)
            {
                return new TimeAttendanceHistoryDto();
            }

            return new TimeAttendanceHistoryDto
            {
                Id = entity.Id,
                Datetime = entity.Datetime,
                UserCodeUpdate = entity.UserCodeUpdate,
                UserCode = entity.UserCode,
                OldValue = entity.OldValue,
                CurrentValue = entity.CurrentValue,
                UpdatedAt = entity.UpdatedAt,
                UpdatedBy = entity.UpdatedBy,
                IsSentToHR = entity.IsSentToHR,
            };
        }

        public static List<TimeAttendanceHistoryDto> ToDtoList(List<TimeAttendanceEditHistory> entity)
        {
            return [.. entity.Select(ToDto)];
        }
    }
}
