using ServicePortal.Domain.Entities;
using ServicePortal.Modules.User.DTO;

namespace ServicePortal.Common.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToDto(User entity)
        {
            return new UserDTO
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                Email = entity.Email,
                RoleId = entity.RoleId,
                IsActive = entity.IsActive,
                DateJoinCompany = entity.DateJoinCompany,
                DateOfBirth = entity.DateOfBirth,
                Phone = entity.Phone,
                Sex = entity.Sex
            };
        }

        public static List<UserDTO> ToDtoList(List<User> users)
        {
            return users.Select(ToDto).ToList();
        }
    }
}
