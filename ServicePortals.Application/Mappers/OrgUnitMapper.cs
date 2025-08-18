//using ServicePortals.Application.Dtos.OrgUnit;
//using ServicePortals.Domain.Entities;

//namespace ServicePortals.Application.Mappers
//{
//    public static class OrgUnitMapper
//    {
//        public static OrgUnitDto ToDto(OrgUnit entity)
//        {
//            if (entity == null)
//            {
//                return new OrgUnitDto();
//            }

//            return new OrgUnitDto
//            {
//                Id = entity.Id,
//                DeptId = entity.DeptId,
//                Name = entity.Name,
//                UnitId = entity.UnitId,
//                ParentOrgUnitId = entity.ParentOrgUnitId,
//                ParentJobTitleId = entity.ParentJobTitleId,
//                ManagerUserCode = entity.ManagerUserCode,
//                DeputyUserCode = entity.DeputyUserCode
//            };
//        }

//        public static List<OrgUnitDto> ToDtoList(List<OrgUnit> entity)
//        {
//            return [.. entity.Select(ToDto)];
//        }
//    }
//}
