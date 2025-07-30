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
using ServicePortals.Application.Mappers;

namespace ServicePortals.Application.Services.OrgUnit
{
    public class OrgUnitService : IOrgUnitService
    {
        private readonly ApplicationDbContext _context;
        private readonly IViclockDapperContext _viclockDapperContext;

        public OrgUnitService (ApplicationDbContext context, IViclockDapperContext viclockDapperContext)
        {
            _context = context;
            _viclockDapperContext = viclockDapperContext;
        }

        /// <summary>
        /// Lấy danh sách phòng ban trong bảng orgunit, where unit_id = 3
        /// </summary>
        public async Task<List<TreeCheckboxResponse>> GetAllDeptOfOrgUnit()
        {
            var results = await _context.OrgUnits.Where(e => e.UnitId == (int)UnitEnum.Phong).ToListAsync();

            var responseList = results.Select(e => new TreeCheckboxResponse
            {
                Id = e.DeptId.ToString(),
                Label = e.Name,
                Type = "department"
            }).ToList();

            return responseList;
        }

        /// <summary>
        /// Lấy org unit theo id
        /// </summary>
        public async Task<Domain.Entities.OrgUnit?> GetOrgUnitById(int id)
        {
            var result = await _context.OrgUnits.FirstOrDefaultAsync(e => e.Id == id);
            
            return result;
        }

        /// <summary>
        /// Lấy tất cả phòng ban và vị trí trong phòng ban đó
        /// </summary>
        public async Task<dynamic?> GetAllDepartmentAndFirstOrgUnit()
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT 
                    pb.Id AS DepartmentId,
                    pb.DeptId,
                    pb.Name AS DepartmentName,
                    org.Id AS OrgUnitId,
                    org.Name AS OrgUnitName
                FROM [{Global.DbWeb}].dbo.org_units pb
                LEFT JOIN [{Global.DbWeb}].dbo.org_units org
                    ON org.ParentOrgUnitId = pb.Id AND org.UnitId = @UnitId
                WHERE pb.DeptId IS NOT NULL AND org.Id IS NOT NULL
                ORDER BY pb.Id, org.Id
            ";

            var rawData = await connection.QueryAsync<dynamic>(sql, new
            {
                UnitId = (int)UnitEnum.To
            });

            var result = rawData
                .GroupBy(x => new { x.DepartmentId, x.DepartmentName })
                .Select(group => new
                {
                    id = group.Key.DepartmentId.ToString(),
                    label = group.Key.DepartmentName,
                    type = "department",
                    children = group.Select(x => new
                    {
                        id = x.OrgUnitId?.ToString() ?? "",
                        label = x.OrgUnitName ?? "",
                        type = "jobtitle"
                    }).ToList()
                })
                .ToList();

            return result;
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

                var sqlUpdate = $@"UPDATE {Global.DbViClock}.dbo.tblNhanVien SET OrgUnitID = @OrgUnitId WHERE NVMaNV IN @UserCodes";

                await connection.ExecuteAsync(sqlUpdate, new
                {
                    OrgUnitId = request.OrgUnitId,
                    UserCodes = request.UserCodes
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
        /// Lấy danh sách id của team (các tổ) và những người dùng chưa được set vị trí (org unit id)
        /// </summary>
        public async Task<object> GetOrgUnitTeamAndUserNotSetOrgUnitWithDept(int departmentId)
        {
            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sqlUserInDeptAndNotSetOrgUnit = $@"
                SELECT
                   NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                WHERE NV.NVMaBP = @departmentId AND NV.OrgUnitID IS NULL AND NV.NVNgayRa > GETDATE()
            ";

            var userData = await connection.QueryAsync<dynamic>(sqlUserInDeptAndNotSetOrgUnit, new { departmentId });

            var orgUnitTeamData = await _context.OrgUnits.Where(e => e.DeptId == departmentId && e.UnitId == (int)UnitEnum.To).ToListAsync();

            List<TreeCheckboxResponse> treeCheckboxResponses = [];

            foreach (var item in orgUnitTeamData)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = item.Id.ToString(),
                    Label = item.Name,
                    Type = "jobtitle"
                });
            }

            foreach (var user in userData)
            {
                treeCheckboxResponses.Add(new TreeCheckboxResponse
                {
                    Id = user.NVMaNV,
                    Label = user.NVHoTen,
                    Type = "user"
                });
            }

            return treeCheckboxResponses;
        }

        /// <summary>
        /// Lấy danh sách vị trí của người dùng thuộc phòng ban, từ 5 trở đi là các vị trí của user
        /// </summary>
        public async Task<List<OrgUnitDto>> GetOrgUnitUserWithDept(int departmentId)
        {
            var results = await _context.OrgUnits.Where(e => e.DeptId == departmentId && e.UnitId >= 5).OrderBy(e => e.UnitId).ToListAsync();

            return OrgUnitMapper.ToDtoList(results);
        }
    }
}
