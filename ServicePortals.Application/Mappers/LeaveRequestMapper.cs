//using ServicePortals.Application.Dtos.ApplicationForm;
//using ServicePortals.Application.Dtos.LeaveRequest;
//using ServicePortals.Domain.Entities;

//namespace ServicePortals.Infrastructure.Mappers
//{
//    public class LeaveRequestMapper
//    {
//        public static LeaveRequestDto ToDto(LeaveRequest entity)
//        {
//            ApplicationFormDto? applicationFormDto = null;

//            if (entity.ApplicationForm != null)
//            {
//                applicationFormDto = new ApplicationFormDto
//                {
//                    Id = entity?.ApplicationForm?.Id,
//                    RequesterUserCode = entity?.ApplicationForm?.RequesterUserCode,
//                    RequestTypeId = entity?.ApplicationForm?.RequestTypeId,
//                    RequestStatusId = entity?.ApplicationForm?.RequestStatusId,
//                    CurrentOrgUnitId = entity?.ApplicationForm?.CurrentOrgUnitId,
//                    CreatedAt = entity?.ApplicationForm?.CreatedAt
//                };
//            }

//            return new LeaveRequestDto
//            {
//                Id = entity?.Id,
//                RequesterUserCode = entity?.RequesterUserCode,
//                UserNameWriteLeaveRequest = entity?.UserNameWriteLeaveRequest,
//                Name = entity?.Name,
//                FromDate = entity?.FromDate,
//                ToDate = entity?.ToDate,
//                TypeLeave = entity?.TypeLeave,
//                TimeLeave = entity?.TimeLeave,
//                Reason = entity?.Reason,
//                DepartmentId = entity?.DepartmentId,
//                Position = entity?.Position,
//                HaveSalary = entity?.HaveSalary,
//                Image = null,
//                CreatedAt = entity?.CreatedAt,
//                Code = entity?.Code,
//                ApplicationFormDto = applicationFormDto,
//                HistoryApplicationForm = entity?.ApplicationForm?.HistoryApplicationForms
//                    .Select(h => new HistoryApplicationForm
//                    {
//                        Id = h.Id,
//                        ApplicationFormId = h.ApplicationFormId,
//                        UserApproval = h.UserApproval,
//                        ActionType = h.ActionType,
//                        Comment = h.Comment,
//                        CreatedAt = h.CreatedAt
//                    })
//                    ?.OrderByDescending(x => x.CreatedAt)
//                    .FirstOrDefault(),
//                Department = entity?.Department?.UnitId == 3 ? entity.Department : null
//            };
//        }

//        public static List<LeaveRequestDto> ToDtoList(List<LeaveRequest> list)
//        {
//            return list.Select(ToDto).ToList();
//        }
//    }
//}
