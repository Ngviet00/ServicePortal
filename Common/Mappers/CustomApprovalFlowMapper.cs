using ServicePortal.Domain.Entities;
using ServicePortal.Modules.CustomApprovalFlow.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class CustomApprovalFlowMapper
    {
        public static CustomApprovalFlowDto ToDto(CustomApprovalFlow entity)
        {
            return new CustomApprovalFlowDto
            {
                Id = entity.Id,
                DepartmentId = entity.DepartmentId,
                TypeCustomApproval = entity.TypeCustomApproval,
                From = entity.From,
                To = entity.To,
                Department = entity.Department,
            };
        }

        public static CustomApprovalFlow ToEntity(CustomApprovalFlowDto dto)
        {
            return new CustomApprovalFlow
            {
                Id = dto.Id ?? -1,
                DepartmentId = dto.DepartmentId,
                TypeCustomApproval = dto.TypeCustomApproval,
                From = dto.From,
                To = dto.To,
            };
        }

        public static List<CustomApprovalFlowDto> ToDtoList(List<CustomApprovalFlow> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<CustomApprovalFlow> ToEntityList(List<CustomApprovalFlowDto> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
