using Microsoft.EntityFrameworkCore;
using ServicePortal.Common;
using ServicePortal.Common.Mappers;
using ServicePortal.Infrastructure.Data;
using ServicePortal.Modules.Deparment.Requests;
using ServicePortal.Modules.Team.DTO;
using ServicePortal.Modules.Team.Interfaces;
using ServicePortal.Modules.Team.Requests;

namespace ServicePortal.Modules.Team.Services
{
    public class Teamservice : ITeamService
    {
        private readonly ApplicationDbContext _context;

        public Teamservice(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResults<TeamDTO>> GetAll(GetAllTeamRequest request)
        {
            string name = request.Name ?? "";
            double pageSize = request.PageSize;
            double page = request.Page;

            var query = _context.Teams.AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(r => r.Name.Contains(name));
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / pageSize);

            // Lấy danh sách team theo trang
            var teams = await query
                .Skip((int)((page - 1) * pageSize))
                .Take((int)pageSize)
                .ToListAsync();

            // Lấy tất cả departmentId từ team
            var departmentIds = teams
                .Where(t => t.DepartmentId.HasValue)
                .Select(t => t.DepartmentId.Value)
                .Distinct()
                .ToList();

            // Lấy tất cả department theo id
            var departments = await _context.Departments
                .Where(d => departmentIds.Contains(d.Id))
                .ToListAsync();

            var departmentMap = departments.ToDictionary(d => d.Id, d => d);

            // Map sang DTO
            var teamDtos = teams.Select(team =>
            {
                var dto = TeamMapper.ToDto(team);

                if (team.DepartmentId.HasValue && departmentMap.TryGetValue(team.DepartmentId.Value, out var department))
                {
                    dto.departmentDto = DepartmentMapper.ToDto(department);
                }
                else
                {
                    dto.departmentDto = null;
                }

                return dto;
            }).ToList();

            // Trả kết quả phân trang
            return new PagedResults<TeamDTO>
            {
                Data = teamDtos,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

        }

        public async Task<Domain.Entities.Team> GetById(int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Team not found!");

            return team;
        }

        public async Task<Domain.Entities.Team> Create(TeamDTO dto)
        {
            var checkExist = await _context.Teams.FirstOrDefaultAsync(e => e.Name == dto.Name);

            if (checkExist != null)
            {
                throw new ValidationException("Team is exists!");
            }

            var team = TeamMapper.ToEntity(dto);

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<Domain.Entities.Team> Update(int id, TeamDTO dto)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Team not found!");

            team = TeamMapper.ToEntity(dto);
            team.Id = id;

            _context.Teams.Update(team);

            await _context.SaveChangesAsync();

            return team;
        }

        public async Task<Domain.Entities.Team> Delete(int id)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(e => e.Id == id) ?? throw new NotFoundException("Team not found!");

            _context.Teams.Remove(team);

            await _context.SaveChangesAsync();

            return team;
        }
    }
}
