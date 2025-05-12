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
                Code = entity.Code,
                Name = entity.Name,
                Email = entity.Email,
                IsActive = entity.IsActive,
                DateJoinCompany = entity.DateJoinCompany,
                DateOfBirth = entity.DateOfBirth,
                Phone = entity.Phone,
                Sex = entity.Sex,
                Position = entity.Position,
                Level = entity.Level,
                LevelParent = entity.LevelParent,
                Department = entity.Department != null ? DepartmentMapper.ToDto(entity.Department) : null,
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
