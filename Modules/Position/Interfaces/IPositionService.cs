namespace ServicePortal.Modules.Position.Interfaces
{
    public interface IPositionService
    {
        Task<List<Domain.Entities.Position>> GetAll();
        Task<Domain.Entities.Position> GetById(int id);
        Task<Domain.Entities.Position> Create(string name, int level);
        Task<Domain.Entities.Position> Update(int id, string name, int level);
        Task<Domain.Entities.Position> Delete(int id);
        Task<Domain.Entities.Position> ForceDelete(int id);
    }
}
