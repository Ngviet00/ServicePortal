using System.Data;
using ServicePortal.Applications.Modules.User.DTO.Responses;
using ServicePortal.Domain.Entities;

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
                Email = entity.Email,
                Roles = entity?.UserRoles != null && entity.UserRoles.All(ur => ur.Role != null)
                ? entity.UserRoles.Select(ur => new Role
                {
                    Id = ur.Role != null ? ur.Role.Id : null,
                    Name = ur.Role != null ? ur.Role.Name : "",
                    Code = ur.Role != null ? ur.Role.Code : ""
                }).ToList()
                : new List<Role>(),
                IsCheckedHaveManageUserTimeKeeping = false
    };
        }

        public static List<UserResponseDto> ToDtoList(List<User> users)
        {
            return users.Select(ToDto).ToList();
        }
    }
}
