using ServicePortals.Application.Dtos.Purchase.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Mappers
{
    public static class PurchaseMapper
    {
        public static PurchaseResponse ToDto(Purchase? x)
        {
            if (x == null)
            {
                return new PurchaseResponse();
            }

            return new PurchaseResponse
            {
                Id = x.Id,
                ApplicationFormId = x.ApplicationFormId,
                //Code = x.Code,
                //UserCode = x.UserCode,
                //UserName = x.UserName,
                DepartmentId = x.DepartmentId,
                RequestedDate = x.RequestedDate,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                DeletedAt = x.DeletedAt,
                OrgUnit = x.OrgUnit == null ? null : new OrgUnit
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
                    RequestType = x.ApplicationForm.RequestType == null ? null : new RequestType
                    {
                        Id = x.ApplicationForm.RequestType.Id,
                        Name = x.ApplicationForm.RequestType.Name,
                        NameE = x.ApplicationForm.RequestType.NameE,
                    },
                    HistoryApplicationForms = x.ApplicationForm.HistoryApplicationForms.OrderByDescending(e => e.CreatedAt).Select(itemHistory => new HistoryApplicationForm
                    {
                        Id = itemHistory.Id,
                        UserNameApproval = itemHistory.UserNameApproval,
                        UserCodeApproval = itemHistory.UserCodeApproval,
                        Action = itemHistory.Action,
                        Note = itemHistory.Note,
                        CreatedAt = itemHistory.CreatedAt
                    }).ToList(),
                    AssignedTasks = x.ApplicationForm.AssignedTasks.ToList(),
                },
                PurchaseDetails = x.PurchaseDetails?.OrderByDescending(h => h.CreatedAt).ToList()
            };
        }

        public static List<PurchaseResponse> ToDtoList(List<Purchase> x)
        {
            return [.. x.Select(ToDto)];
        }
    }
}
