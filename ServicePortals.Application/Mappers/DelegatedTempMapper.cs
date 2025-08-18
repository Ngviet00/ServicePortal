//using ServicePortals.Application.Dtos.DelegatedTemp;
//using ServicePortals.Domain.Entities;

//namespace ServicePortals.Application.Mappers
//{
//    public static class DelegatedTempMapper
//    {
//        public static DelegatedTempDto ToDto(Delegation entity)
//        {
//            return new DelegatedTempDto
//            {
//                Id = entity.Id,
//                MainOrgUnitId = entity.MainOrgUnitId,
//                TempUserCode = entity.TempUserCode,
//                RequestTypeId = entity.RequestTypeId,
//                FromDate = entity.FromDate,
//                ToDate = entity.ToDate,
//                IsActive = entity.IsActive,
//                IsPermanent = entity.IsPermanent,
//            };
//        }

//        public static List<DelegatedTempDto> ToDtoList(List<Delegation> delegatedTempDto)
//        {
//            return delegatedTempDto.Select(ToDto).ToList();
//        }
//    }
//}
