namespace ServicePortals.Application.Interfaces.CostCenter
{
    public interface ICostCenterService
    {
        Task<List<Domain.Entities.CostCenter>> GetAll();
    }
}
