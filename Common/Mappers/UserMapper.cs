using System.Data;
using ServicePortal.Domain.Entities;
using ServicePortal.Modules.User.DTO.Responses;

namespace ServicePortal.Common.Mappers
{
    public static class UserMapper
    {
        public static UserResponseDto ToDto(User entity)
        {
            return new UserResponseDto
            {
                Id = entity.Id,
                UserCode = entity.UserCode,
                PositionId = entity.PositionId,
                Password = entity.Password,
                IsActive = entity.IsActive,
                IsChangePassword = entity.IsChangePassword,
                Roles = entity?.UserRoles != null && entity.UserRoles.All(ur => ur.Role != null)
                ? entity.UserRoles.Select(ur => new Role
                {
                    Id = ur.Role != null ? ur.Role.Id : null,
                    Name = ur.Role != null ? ur.Role.Name : "",
                    Code = ur.Role != null ? ur.Role.Code : ""
                }).ToList()
                : new List<Role>(),
            };
        }

        public static List<UserResponseDto> ToDtoList(List<User> users)
        {
            return users.Select(ToDto).ToList();
        }
    }
}
