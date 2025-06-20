﻿using System.Globalization;
using ServicePortal.Applications.Modules.LeaveRequest.DTO;
using ServicePortal.Domain.Entities;

namespace ServicePortal.Common.Mappers
{
    public class LeaveRequestMapper
    {
        public static LeaveRequestDto ToDto(LeaveRequest entity, ApprovalRequest? approvalRequest = null, ApprovalAction? approvalAction = null)
        {
            return new LeaveRequestDto
            {
                Id = entity.Id,
                WriteLeaveUserCode = entity.WriteLeaveUserCode,
                WriteLeaveName = entity.WriteLeaveName,
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
                ApprovalRequest = approvalRequest,
                ApprovalAction = approvalAction
            };
        }

        public static LeaveRequest ToEntity(LeaveRequestDto dto)
        {
            return new LeaveRequest
            {
                WriteLeaveUserCode = dto.WriteLeaveUserCode,
                WriteLeaveName = dto.WriteLeaveName,
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

        public static List<LeaveRequestDto> ToDtoList(List<(LeaveRequest, ApprovalRequest?, ApprovalAction?)> list)
        {
            return list.Select(x => ToDto(x.Item1, x.Item2, x.Item3)).ToList();
        }

        public static List<LeaveRequest> ToEntityList(List<LeaveRequestDto> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
