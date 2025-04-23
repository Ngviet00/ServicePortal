using ServicePortal.Common;
using ServicePortal.Modules.User.DTO;
using ServicePortal.Modules.User.Requests;

namespace ServicePortal.Modules.User.Interfaces
{
    public interface IUserService
    {
        Task<PagedResults<UserDTO>> GetAll(GetAllUserRequest request);
        Task<UserDTO> GetById(Guid id);
        Task<UserDTO> GetByCode(string code);
        //Task<UserDTO> Update(Guid id, UpdateUserRequest request);
        Task<UserDTO> Delete(Guid id);
        Task<UserDTO> ForceDelete(Guid id);
    }
}
