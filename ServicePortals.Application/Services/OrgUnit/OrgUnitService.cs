using Microsoft.Data.SqlClient;
using System.Data;
using ServicePortals.Application.Interfaces.OrgUnit;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using Dapper;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.OrgUnit.Requests;
using Serilog;
using ServicePortals.Domain.Enums;
using ServicePortals.Shared.SharedDto;
using ServicePortals.Application.Dtos.OrgUnit;
using Entities = ServicePortals.Domain.Entities;
using ServicePortals.Shared.Exceptions;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly ApplicationDbContext _context;

        public OrgUnitService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy tất cả department
        /// </summary>
        public async Task<List<Entities.OrgUnit>> GetAllDepartments()
        {
            var results = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department).ToListAsync();

            return results;
        }

        /// <summary>
        /// Lấy danh sách id của team (các tổ) và những người dùng chưa được set vị trí tổ chức
        /// </summary>
        public async Task<object> GetTeamByDeptIdAndUserNotSetOrgPositionId(int departmentId)
        {
            var department = await _context.OrgUnits.Where(e => e.Id == departmentId).FirstOrDefaultAsync() ?? throw new NotFoundException("Department not found");

            List<TreeCheckboxResponse> treeCheckboxResponses = [];

            //lấy danh sách các team thuộc bộ phận
            var teams = await _context.OrgUnits.Where(e => e.ParentOrgUnitId == departmentId && e.UnitId == (int)UnitEnum.Team).ToListAsync();

            foreach (var item in teams)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = item.Id.ToString(),
                    Label = item.Name,
                    Type = "team"
                });
            }

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            //case user thuộc bộ phận, k thuộc tổ nào
            var positionIds = await _context.Positions.Where(e => e.OrgUnitId == departmentId).Select(e => e.Id).ToListAsync();

            var sqlUserInDepartment = $@"
                SELECT
                   NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                WHERE ViTriToChucId IN @positionIds AND NV.NVNgayRa > GETDATE()
            ";

            var resultsUserInDepartment = await connection.QueryAsync<dynamic>(sqlUserInDepartment, new { positionIds });

            foreach (var item in resultsUserInDepartment)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = item.NVMaNV,
                    Label = item.NVHoTen,
                    Type = "user"
                });
            }

            //case user chưa được set vị trí tổ chức Id
            var sqlUserNotSetOrgPositionId = $@"
                SELECT
                   NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblBoPhan AS BP
                INNER JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV ON NV.NVMaBP = BP.BPMa
                WHERE NV.NVNgayRa > GETDATE() AND BP.BPTen = @departmentName AND NV.ViTriToChucId IS NULL
            ";

            var resultsUserNotSetOrgPositionId = await connection.QueryAsync<dynamic>(sqlUserNotSetOrgPositionId, new { departmentName = department.Name });

            foreach (var item in resultsUserNotSetOrgPositionId)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = item.NVMaNV,
                    Label = item.NVHoTen,
                    Type = "user"
                });
            }

            return treeCheckboxResponses;
        }
        public async Task<object> GetListUserByTeamId(int teamId)
        {
            var positions = await _context.Positions.Where(e => e.OrgUnitId == teamId).Select(e => e.Id).ToListAsync();

            List<TreeCheckboxResponse> treeCheckboxResponses = [];

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT 
                    NV.NVMaNV, vs_new.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                WHERE ViTriToChucId IN @positions AND NV.NVNgayRa > GETDATE()
            ";

            var results = await connection.QueryAsync<dynamic>(sql, new { positions });

            foreach (var item in results)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = item.NVMaNV,
                    Label = item.NVHoTen,
                    Type = "user"
                });
            }

            return treeCheckboxResponses;
        }

        /// <summary>
        /// Lưu thay đổi vị trí người dùng
        /// </summary>
        public async Task<bool> SaveChangeUserOrgUnit(SaveChangeOrgUnitUserRequest request)
        {
            try
            {
                var connection = (SqlConnection)_context.CreateConnection();

                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                var sqlUpdate = $@"UPDATE {Global.DbViClock}.dbo.tblNhanVien SET ViTriToChucId = @ViTriToChucId WHERE NVMaNV IN @UserCodes";

                await connection.ExecuteAsync(sqlUpdate, new
                {
                    request.ViTriToChucId,
                    request.UserCodes
                });

                return true;
            }
            catch (Exception ex)
            {
                Log.Error($"Error can not change org unit user, error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy tất cả phòng ban và team thuộc phòng ban đó
        /// </summary>
        public async Task<dynamic?> GetDepartmentAndChildrenTeam()
        {
            var orgUnits = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Department || e.UnitId == (int)UnitEnum.Team).ToListAsync();

            var dict = orgUnits.ToDictionary(
                x => x.Id,
                x => new
                {
                    id = x.Id.ToString(),
                    label = x.Name,
                    type = x.UnitId == (int)UnitEnum.Department ? "department" : "team",
                    children = new List<object>()
                } as dynamic
            );

            List<dynamic> roots = [];

            foreach (var item in orgUnits)
            {
                if (item.ParentOrgUnitId.HasValue && dict.TryGetValue(item.ParentOrgUnitId.Value, out var parent))
                {
                    parent.children.Add(dict[item.Id]);
                }
                else
                {
                    roots.Add(dict[item.Id]);
                }
            }

            return roots;
        }

        ///// <summary>
        ///// Lấy danh sách phòng ban trong bảng orgunit, where unit_id = 3, đã format dữ liệu để hiển thị tree checkbox các màn
        ///// </summary>
        //public async Task<List<TreeCheckboxResponse>> GetAllDeptOfOrgUnit()
        //{
        //    var results = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Phong).ToListAsync();

        //    var responseList = results.Select(e => new TreeCheckboxResponse
        //    {
        //        Id = e.DeptId.ToString(),
        //        Label = e.Name,
        //        Type = "department"
        //    }).ToList();

        //    return responseList;
        //}

        ///// <summary>
        ///// Lấy org unit theo id
        ///// </summary>
        //public async Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id)
        //{
        //    var result = await _context.OrgUnits.FirstOrDefaultAsync(e => e.Id == id);

        //    return result;
        //}

        ///// <summary>
        ///// Lấy danh sách vị trí của người dùng thuộc phòng ban, từ 5 trở đi là các vị trí của user
        ///// </summary>
        //public async Task<List<OrgUnitDto>> GetOrgUnitUserWithDept(int departmentId)
        //{
        //    var results = await _context.OrgUnits.Where(e => e.DeptId == departmentId && e.UnitId >= 5).OrderBy(e => e.UnitId).ToListAsync();

        //    return OrgUnitMapper.ToDtoList(results);
        //}
    }
}
