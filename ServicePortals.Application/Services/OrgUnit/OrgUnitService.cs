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
using Entities = ServicePortals.Domain.Entities;
using ServicePortals.Shared.Exceptions;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;

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
            var orgPositionIds = await _context.OrgPositions.Where(e => e.OrgUnitId == departmentId).Select(e => e.Id).ToListAsync();

            var sqlUserInDepartment = $@"
                SELECT
                   NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                WHERE ViTriToChucId IN @orgPositionIds AND NV.NVNgayRa > GETDATE()
            ";

            var resultsUserInDepartment = await connection.QueryAsync<dynamic>(sqlUserInDepartment, new { orgPositionIds });

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
            var orgPositions = await _context.OrgPositions.Where(e => e.OrgUnitId == teamId).Select(e => e.Id).ToListAsync();

            List<TreeCheckboxResponse> treeCheckboxResponses = [];

            var connection = (SqlConnection)_context.CreateConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            var sql = $@"
                SELECT 
                    NV.NVMaNV, {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS NVHoTen 
                FROM {Global.DbViClock}.dbo.tblNhanVien AS NV
                WHERE ViTriToChucId IN @orgPositions AND NV.NVNgayRa > GETDATE()
            ";

            var results = await connection.QueryAsync<dynamic>(sql, new { orgPositions });

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

                var sqlUpdate = $@"UPDATE {Global.DbViClock}.dbo.tblNhanVien SET ViTriToChucId = @OrgPositionId WHERE NVMaNV IN @UserCodes";

                await connection.ExecuteAsync(sqlUpdate, new
                {
                    request.OrgPositionId,
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

        public async Task<List<Entities.OrgUnit>> GetAll(Expression<Func<Entities.OrgUnit, bool>>? predicate = null, int? departmentId = null)
        {
            var query = _context.OrgUnits.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (departmentId != null)
            {
                query = query.Where(e => e.ParentOrgUnitId == departmentId || (e.ParentOrgUnit != null && e.ParentOrgUnit.ParentOrgUnitId == departmentId));
            }

            var results =  await query
                .Select(e => new Entities.OrgUnit
                {
                    Id = e.Id,
                    Name = e.Name,
                    ParentOrgUnitId = e.ParentOrgUnitId,
                    UnitId = e.UnitId,
                    Unit = e.Unit == null ? null : new Domain.Entities.Unit
                    {
                        Id = e.Unit.Id,
                        Name = e.Unit.Name
                    },
                    ParentOrgUnit = e.ParentOrgUnit == null ? null : new Domain.Entities.OrgUnit
                    {
                        Id = e.ParentOrgUnit.Id,
                        Name = e.ParentOrgUnit.Name,
                        ParentOrgUnitId = e.ParentOrgUnit.ParentOrgUnitId
                    }
                })
                .ToListAsync();

            return results;
        }

        public async Task<object> SaveOrUpdateOrgUnit(SaveOrUpdateOrgUnitRequest request)
        {
            if (request.Id == null)
            {
                var newItem = new Domain.Entities.OrgUnit
                {
                    Name = request.Name,
                    ParentOrgUnitId = request.ParentOrgUnitId,
                    UnitId = request.UnitId,
                };

                _context.OrgUnits.Add(newItem);
            }
            else
            {
                var item = await _context.OrgUnits.FirstOrDefaultAsync(e => e.Id == request.Id) ?? throw new NotFoundException("Org unit not found. check again");

                item.Name = request.Name;
                item.ParentOrgUnitId = request.ParentOrgUnitId;
                item.UnitId = request.UnitId;

                _context.OrgUnits.Update(item);
            }

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<object> Delete(int id)
        {
            await _context.OrgUnits.Where(e => e.Id == id).ExecuteDeleteAsync();

            return true;
        }

        public async Task<bool> SaveChangeOrgUnitManyUser(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("No file uploaded");
            }

            var connection = (SqlConnection)_context.CreateConnection();
            await connection.OpenAsync();

            using var transaction = connection.BeginTransaction();

            try
            {
                using var workbook = new XLWorkbook(file.OpenReadStream());
                var worksheet = workbook.Worksheet(1);

                Helper.ValidateExcelHeader(worksheet, ["user_code", "org_position_id"]);

                var rows = worksheet?.RangeUsed()?.RowsUsed().Skip(1);

                var cases = new List<string>();
                var userCodes = new List<string>();

                var userCodesCheck = new List<string>();
                var orgPositionIdCheck = new List<int?>();

                if (rows != null)
                {
                    if (!rows.Any())
                    {
                        throw new ValidationException("Không có dữ liệu nào trong file excel");
                    }

                    foreach (var row in rows)
                    {
                        string userCode = row.Cell(1).GetValue<string>();
                        int orgPositionId = row.Cell(2).GetValue<int>();

                        if (string.IsNullOrWhiteSpace(userCode) || orgPositionId <= 0)
                        {
                            throw new ValidationException("Dữ liệu sai định dạng, vui lòng kiểm tra lại");
                        }

                        cases.Add($"WHEN '{userCode}' THEN {orgPositionId}");
                        userCodes.Add($"'{userCode}'");

                        userCodesCheck.Add(userCode);
                        orgPositionIdCheck.Add(orgPositionId);
                    }
                }

                var orgPositionIdDb = await connection.QueryAsync<int>(
                    @"SELECT Id 
                      FROM org_positions 
                      WHERE Id IN @ids",
                    new { ids = orgPositionIdCheck },
                    transaction: transaction
                );

                if (orgPositionIdDb.Count() != orgPositionIdCheck.Distinct().Count())
                {
                    throw new ValidationException("Dữ liệu sai định dạng, vui lòng kiểm tra lại");
                }

                var numberUsersExist = await connection.ExecuteScalarAsync<int>($@"SELECT COUNT(1) FROM {Global.DbViClock}.dbo.tblNhanVien WHERE NVMaNV IN @userCodesCheck", new { userCodesCheck }, transaction: transaction);

                if (numberUsersExist != userCodesCheck.Distinct().Count())
                {
                    throw new ValidationException("Dữ liệu sai định dạng, vui lòng kiểm tra lại");
                }

                var sql = $@"
                    UPDATE {Global.DbViClock}.dbo.tblNhanVien
                    SET ViTriToChucId = CASE NVMaNV
                        {string.Join("\n", cases)}
                    END
                    WHERE NVMaNV IN ({string.Join(",", userCodes)})";

                var affected = await connection.ExecuteAsync(sql, transaction: transaction);

                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error($"Error import excel, ex: {ex.Message}");

                throw new ValidationException("Dữ liệu sai định dạng, vui lòng kiểm tra lại");
            }
        }
    }
}
