using ServicePortal.Domain.Entities;
using ServicePortal.Modules.CustomApprovalFlow.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class CustomApprovalFlowMapper
    {
        public static CustomApprovalFlowDTO ToDto(CustomApprovalFlow entity)
        {
            return new CustomApprovalFlowDTO
            {
                Id = entity.Id,
                DepartmentId = entity.DepartmentId,
                TypeCustomApproval = entity.TypeCustomApproval,
                From = entity.From,
                To = entity.To,
                Department = entity.Department,
            };
        }

        public static CustomApprovalFlow ToEntity(CustomApprovalFlowDTO dto)
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

        public static List<CustomApprovalFlowDTO> ToDtoList(List<CustomApprovalFlow> entities)
        {
            return entities.Select(ToDto).ToList();
        }

        public static List<CustomApprovalFlow> ToEntityList(List<CustomApprovalFlowDTO> dtos)
        {
            return dtos.Select(ToEntity).ToList();
        }
    }
}
