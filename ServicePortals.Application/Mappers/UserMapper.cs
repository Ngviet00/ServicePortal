using System.Data;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Infrastructure.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToDto(User entity)
        {
            return new UserResponse
            {
                Id = entity.Id,
                UserCode = entity.UserCode,
                PositionId = entity.PositionId,
                Password = entity.Password,
                IsActive = entity.IsActive,
                IsChangePassword = entity.IsChangePassword,
                Email = entity.Email,
                Roles = entity?.UserRoles != null && entity.UserRoles.All(ur => ur.Role != null)
                ? [.. entity.UserRoles.Select(ur => new Role
                {
                    Id = ur.Role != null ? ur.Role.Id : null,
                    Name = ur.Role != null ? ur.Role.Name : "",
                    Code = ur.Role != null ? ur.Role.Code : ""
                })]
                : [],
                IsCheckedHaveManageUserTimeKeeping = false
    };
        }

        public static List<UserResponse> ToDtoList(List<User> users)
        {
            return [.. users.Select(ToDto)];
        }
    }
}
