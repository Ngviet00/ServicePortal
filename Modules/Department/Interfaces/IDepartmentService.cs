﻿using ServicePortal.Common;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Requests;

namespace ServicePortal.Modules.Deparment.Interfaces
{
    public interface IDepartmentService
    {
        Task<PagedResults<DepartmentDTO>> GetAll(GetAllDepartmentRequest request);
        Task<List<Domain.Entities.Department>> GetParentDepartment();
        Task<Domain.Entities.Department> GetById(int id);
        Task<Domain.Entities.Department> Create(DepartmentDTO dto);
        Task<Domain.Entities.Department> Update(int id, DepartmentDTO dto);
        Task<Domain.Entities.Department> Delete(int id);
    }
}
