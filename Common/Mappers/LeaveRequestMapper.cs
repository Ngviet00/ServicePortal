using System.Globalization;
using ServicePortal.Domain.Entities;
using ServicePortal.Modules.LeaveRequest.DTO;

namespace ServicePortal.Common.Mappers
{
    public class LeaveRequestMapper
    {
        public static LeaveRequestDto ToDto(LeaveRequest entity, ApprovalAction? approvalAction = null)
        {
            return new LeaveRequestDto
            {
                Id = entity.Id,
                WriteLeaveUserCode = entity.WriteLeaveUserCode,
                RequesterUserCode = entity.RequesterUserCode,
                Name = entity.Name,
                FromDate = entity.FromDate.ToString(),
                ToDate = entity.ToDate.ToString(),
                TypeLeave = entity?.TypeLeave,
                TimeLeave = entity?.TimeLeave,
                Reason = entity?.Reason,
                Department = entity?.Department,
                Position = entity?.Position,
                HaveSalary = entity?.HaveSalary,
                Image = entity?.Image,
                CreatedAt = entity?.CreatedAt,
                ApprovalAction = approvalAction
            };
        }

        public static LeaveRequest ToEntity(LeaveRequestDto dto)
        {
            return new LeaveRequest
            {
                WriteLeaveUserCode = dto.WriteLeaveUserCode,
                RequesterUserCode = dto.RequesterUserCode,
                Name = dto.Name,
                FromDate = DateTimeOffset.ParseExact(dto?.FromDate ?? "", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                ToDate = DateTimeOffset.ParseExact(dto?.ToDate ?? "", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                TypeLeave = dto?.TypeLeave,
                TimeLeave = dto?.TimeLeave,
                Reason = dto?.Reason,
                Department = dto?.Department,
                Position = dto?.Position,
                HaveSalary = dto?.HaveSalary,
                Image = dto?.Image,
                CreatedAt = DateTimeOffset.Now
            };
        }

        public static List<LeaveRequestDto> ToDtoList(List<(LeaveRequest, ApprovalAction?)> list)
        {
            return list.Select(x => ToDto(x.Item1, x.Item2)).ToList();
        }

        public static List<LeaveRequest> ToEntityList(List<LeaveRequestDto> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
