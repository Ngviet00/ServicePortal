using System.Data;
using ServicePortals.Application.Dtos.User.Responses;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Infrastructure.Mappers
{
    public static class UserMapper
    {
        public static UserResponse ToDto(User entity)
        {
            if (entity == null)
            {
                return new UserResponse();
            }

            return new UserResponse
            {
                Id = entity.Id,
                UserCode = entity.UserCode,
                Password = entity.Password,
                IsActive = entity.IsActive,
                IsChangePassword = entity.IsChangePassword,
                Email = entity.Email,
                DateOfBirth = entity.DateOfBirth,
                Phone = entity.Phone
            };
        }

        public static List<UserResponse> ToDtoList(List<User> users)
        {
            return [.. users.Select(ToDto)];
        }
    }
}
