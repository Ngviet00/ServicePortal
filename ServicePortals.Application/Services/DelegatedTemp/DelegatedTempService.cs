using Dapper;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.DelegatedTemp;
using ServicePortals.Application.Dtos.DelegatedTemp.Requests;
using ServicePortals.Application.Dtos.DelegatedTemp.Responses;
using ServicePortals.Application.Interfaces.DelegatedTemp;
using ServicePortals.Application.Mappers;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;

namespace ServicePortals.Application.Services.DelegatedTemp
{
    public class DelegatedTempService : IDelegatedTempService
    {
        private readonly ApplicationDbContext _context;

        public DelegatedTempService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DelegatedTempDto> AddNew(CreateDelegatedTempRequest request)
        {
            var mainOrgUnitId = request.MainOrgUnitId;
            var userCodeMain = request.MainUserCode;
            var userCodeTemp = request.TempUserCode;

            if (mainOrgUnitId == null)
            {
                var connection = _context.Database.GetDbConnection();

                mainOrgUnitId = await connection.QueryFirstOrDefaultAsync<int>($@"SELECT OrgUnitId FROM vs_new.dbo.tblNhanVien AS NV WHERE NV.NVMaNV = @MainUserCode", new { MainUserCode = userCodeMain });
            }

            var newItem = new Domain.Entities.DelegatedTemp
            {
                MainOrgUnitId = mainOrgUnitId,
                TempUserCode = userCodeTemp,
                RequestTypeId = request.RequestTypeId,
                IsActive = true,
                IsPermanent = true
            };

            _context.DelegatedTemps.Add(newItem);

            await _context.SaveChangesAsync();

            return DelegatedTempMapper.ToDto(newItem);
        }

        public async Task<DelegatedTempDto> Delete(DelegatedTempDto request)
        {
            var item = await _context.DelegatedTemps.FirstOrDefaultAsync(e => e.MainOrgUnitId == request.MainOrgUnitId && e.TempUserCode == request.TempUserCode);

            _context.DelegatedTemps.Remove(item);

            await _context.SaveChangesAsync();

            return DelegatedTempMapper.ToDto(item);
        }

        public async Task<List<GetAllDelegatedTempResponse>> GetAll(DelegatedTempDto request)
        {
            var delegatedTemp = await _context.DelegatedTemps.Where(e => e.RequestTypeId == request.RequestTypeId).ToListAsync();

            var sql = $@"
            SELECT
                  DT.MainOrgUnitId, 
                  ORG.Name AS OrgUnitName, 
                  {Global.DbViClock}.dbo.funTCVN2Unicode(NV1.NVHoTen) AS MainUser,
                  NV1.NVMaNV AS MainUserCode, 
                  {Global.DbViClock}.dbo.funTCVN2Unicode(NV.NVHoTen) AS TempUser,
                  NV.NVMaNV AS TempUserCode
                FROM
                  {Global.DbWeb}.[dbo].[delegated_temp] AS DT
                  INNER JOIN {Global.DbWeb}.dbo.org_units AS ORG ON DT.MainOrgUnitId = ORG.Id
                  LEFT JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV ON DT.TempUserCode = NV.NVMaNV
                  LEFT JOIN {Global.DbViClock}.dbo.tblNhanVien AS NV1 ON DT.MainOrgUnitId = NV1.OrgUnitID
                WHERE
                  DT.RequestTypeId = @RequestTypeId";

            var connection = _context.Database.GetDbConnection();

            var results = await connection.QueryAsync<GetAllDelegatedTempResponse>(sql, new { RequestTypeId = request.RequestTypeId });

            return (List<GetAllDelegatedTempResponse>)results;
        }
    }
}
