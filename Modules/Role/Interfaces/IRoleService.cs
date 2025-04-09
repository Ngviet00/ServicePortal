namespace ServicePortal.Modules.Role.Interfaces
{
    public interface IRoleService
    {
        Task<List<Domain.Entities.Role>> GetAll();
        Task<Domain.Entities.Role> GetById(int id);
        Task<Domain.Entities.Role> Create(string name);
        Task<Domain.Entities.Role> Update(int id, string name);
        Task<Domain.Entities.Role> Delete(int id);
    }
}
