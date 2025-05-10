using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.DTO;
using ServicePortal.Modules.Deparment.Interfaces;
using ServicePortal.Modules.Deparment.Requests;
using ServicePortal.Modules.Department.DTO;

namespace ServicePortal.Modules.Deparment.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationDbContext _context;

        public DepartmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<DepartmentDTO>> GetAll(GetAllDepartmentRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Departments.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            var departments = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            var parentIds = departments
                .Where(d => d.ParentId.HasValue)
                .Select(d => d.ParentId.Value)
                .Distinct()
                .ToList();

            var parentDepartments = await _context.Departments
                .Where(d => parentIds.Contains(d.Id))
                .ToListAsync();
            var parentMap = parentDepartments.ToDictionary(p => p.Id, p => p);

            var departmentDtos = departments.Select(d =>
            {
                var dto = DepartmentMapper.ToDto(d);

                if (d.ParentId.HasValue && parentMap.TryGetValue(d.ParentId.Value, out var parent))
                {
                    dto.Parent = DepartmentMapper.ToDto(parent);
                }
                else
                {
                    dto.Parent = null;
                }

                return dto;
            }).ToList();

            return new PagedResults<DepartmentDTO>
            {
                Data = departmentDtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }

        public async Task<List<Domain.Entities.Department>> GetParentDepartment()
        {
            var deparment = await _context.Departments.Where(e => e.ParentId == null).ToListAsync();

            return deparment;
        }

        public async Task<List<DepartmentTreeDTO>> GetDepartmentWithChildrenDepartmentAndPosition()
        {
            var allDepartments = await _context.Departments.ToListAsync();

            //var allPositionsByDepartment = await _context.Positions
            //    .Where(p => p.Level >= 1)
            //    .GroupBy(p => p.DepartmentId ?? -1)
            //    .ToDictionaryAsync(
            //        g => g.Key,
            //        g => g.Select(p => new PositionDTO
            //        {
            //            Id = p.Id,
            //            Name = p.Name,
            //            Title = p.Title,
            //            DepartmentId = p.DepartmentId,
            //            Level = p.Level
            //        }).ToList()
               //);

            var departmentDtoMap = allDepartments.ToDictionary(
                d => d.Id
                //d => 
                //{
                //    allPositionsByDepartment.TryGetValue(d.Id, out var positions);
                //    return new DepartmentTreeDTO
                //    {
                //        Id = d.Id,
                //        Name = d.Name,
                //        ParentId = d.ParentId,
                //        //Positions = positions ?? new List<PositionDTO>(),
                //        Childrens = new List<DepartmentTreeDTO>()
                //    };
                //}
            );

            foreach (var department in allDepartments)
            {
                if (department.ParentId.HasValue && departmentDtoMap.TryGetValue(department.ParentId.Value, out var parentDto))
                {
                    //parentDto.Childrens.Add(departmentDtoMap[department.Id]);
                }
            }

            var rootDepartments = departmentDtoMap.Values.Where(d => !d.ParentId.HasValue).ToList();

            return null;
        }

        public async Task<Domain.Entities.Department> GetById(int id)
        {
            var deparment = await _context.Departments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            return deparment;
        }

        public async Task<Domain.Entities.Department> Create(DepartmentDTO dto)
        {
            var checkExist = await _context.Departments.FirstOrDefaultAsync(e => e.Name == dto.Name);

            if (checkExist != null)
            {
                throw new ValidationException("Department is exists!");
            }

            var deparment = DepartmentMapper.ToEntity(dto);

            _context.Departments.Add(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }

        public async Task<Domain.Entities.Department> Update(int id, DepartmentDTO dto)
        {
            var deparment = await _context.Departments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            deparment = DepartmentMapper.ToEntity(dto);
            deparment.Id = id;

            _context.Departments.Update(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }

        public async Task<Domain.Entities.Department> Delete(int id)
        {
            var deparment = await _context.Departments.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Deparment not found!");

            _context.Departments.Remove(deparment);

            await _context.SaveChangesAsync();

            return deparment;
        }
    }
}
