using ServicePortals.Application.Dtos.ITForm.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Mappers
{
    public static class ITFormMapper
    {
        public static ITFormResponse ToDto(ITForm? x)
        {
            if (x == null)
            {
                return new ITFormResponse();
            }

            return new ITFormResponse
            {
                Id = x.Id,
                ApplicationFormId = x.ApplicationFormId,
                Code = x.Code,
                UserCodeRequestor = x.UserCodeRequestor,
                UserNameRequestor = x.UserNameRequestor,
                UserCodeCreated = x.UserCodeCreated,
                UserNameCreated = x.UserNameCreated,
                DepartmentId = x.DepartmentId,
                Email = x.Email,
                Position = x.Position,
                Reason = x.Reason,
                PriorityId = x.PriorityId,
                NoteManagerIT = x.NoteManagerIT,
                RequestDate = x.RequestDate,
                RequiredCompletionDate = x.RequiredCompletionDate,
                TargetCompletionDate = x.TargetCompletionDate,
                ActualCompletionDate = x.ActualCompletionDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                Priority = x.Priority == null ? null : new Domain.Entities.Priority
                {
                    Id = x.Priority.Id,
                    Name = x.Priority.Name,
                    NameE = x.Priority.NameE
                },
                OrgUnit = x.OrgUnit == null ? null : new Domain.Entities.OrgUnit
                {
                    Id = x.OrgUnit.Id,
                    Name = x.OrgUnit.Name,
                    ParentOrgUnitId = x.OrgUnit.ParentOrgUnitId
                },
                ApplicationForm = x.ApplicationForm == null ? null : new ApplicationForm
                {
                    Id = x.ApplicationForm.Id,
                    UserCodeRequestor = x.ApplicationForm.UserCodeRequestor,
                    RequestStatusId = x.ApplicationForm.RequestStatusId,
                    RequestTypeId = x.ApplicationForm.RequestTypeId,
                    OrgPositionId = x.ApplicationForm.OrgPositionId,
                    CreatedAt = x.ApplicationForm.CreatedAt,
                    RequestStatus = x.ApplicationForm.RequestStatus == null ? null : new RequestStatus
                    {
                        Id = x.ApplicationForm.RequestStatus.Id,
                        Name = x.ApplicationForm.RequestStatus.Name,
                        NameE = x.ApplicationForm.RequestStatus.NameE,
                    },
                    RequestType = x.ApplicationForm.RequestType == null ? null : new Domain.Entities.RequestType
                    {
                        Id = x.ApplicationForm.RequestType.Id,
                        Name = x.ApplicationForm.RequestType.Name,
                        NameE = x.ApplicationForm.RequestType.NameE,
                    },
                    HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(e => e.CreatedAt).ToList()
                },
                ItFormCategories = x.ItFormCategories.Select(ift => new ITFormCategory
                {
                    Id = ift.Id,
                    ITCategoryId = ift.ITCategoryId,
                    ITFormId = ift.ITFormId,
                    ITCategory = ift.ITCategory == null ? null : new ITCategory
                    {
                        Id = ift.ITCategory.Id,
                        Name = ift.ITCategory.Name,
                        Code = ift.ITCategory.Code
                    }
                }).ToList(),
            };
        }

        public static List<ITFormResponse> ToDtoList(List<ITForm> x)
        {
            return [.. x.Select(ToDto)];
        }
    }
}
