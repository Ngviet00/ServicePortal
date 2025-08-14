//using ServicePortals.Application.Dtos.MemoNotification;
//using ServicePortals.Domain.Entities;

//namespace ServicePortals.Infrastructure.Mappers
//{
//    public static class MemoNotifyMapper
//    {
//        public static MemoNotificationDto ToDto(MemoNotification? entity)
//        {
//            return new MemoNotificationDto
//            {
//                Id = entity?.Id,
//                Title = entity?.Title,
//                Content = entity?.Content,
//                FromDate = entity?.FromDate,
//                ToDate = entity?.ToDate,
//                UserCodeCreated = entity?.UserCodeCreated,
//                CreatedBy = entity?.CreatedBy,
//                CreatedAt = entity?.CreatedAt,
//                UpdatedBy = entity?.UpdatedBy,
//                UpdatedAt = entity?.UpdatedAt,
//                Priority = entity?.Priority,
//                Status = entity?.Status,
//                ApplyAllDepartment = entity?.ApplyAllDepartment,
//            };
//        }

//        public static List<MemoNotificationDto> ToDtoList(List<MemoNotification> memoNotifications)
//        {
//            return memoNotifications.Select(ToDto).ToList();
//        }
//    }
//}
