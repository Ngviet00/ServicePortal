using System.Data;
using System.Security.Claims;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.Approval.Request;
using ServicePortals.Application.Dtos.Approval.Response;
using ServicePortals.Application.Interfaces.Approval;
using ServicePortals.Infrastructure.Data;
using ServicePortals.Infrastructure.Helpers;
using ServicePortals.Shared.Exceptions;

namespace ServicePortals.Application.Services.Approval
{
    public class ApprovalService : IApprovalService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApprovalService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PagedResults<PendingApproval>> ListWaitApprovals(ListWaitApprovalRequest request)
        {
            var userClaims = _httpContextAccessor.HttpContext.User;
            var roleClaims = userClaims.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool isHR = roleClaims?.Contains("HR") == true && permissionClaims?.Contains("leave_request.hr_management_leave_request") == true;

            if (request.OrgPositionId == 0)
            {
                throw new ValidationException(Global.UserNotSetInformation);
            }

            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@IsHR", isHR, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@DepartmentId", request.DepartmentId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OrgPositionId", request.OrgPositionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RequestTypeId", request.RequestTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<PendingApproval>(
                    "dbo.Approval_GET_GetListWaitApproval",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<PendingApproval>
            {
                Data = (List<PendingApproval>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<CountWaitApprovalAndAssignedInSidebarResponse> CountWaitAprrovalAndAssignedInSidebar(CountWaitAprrovalAndAssignedInSidebarRequest request)
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            var roleClaims = userClaims?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var permissionClaims = userClaims?.Claims.Where(c => c.Type == "permission").Select(c => c.Value).ToHashSet(StringComparer.OrdinalIgnoreCase);
            bool isHR = roleClaims?.Contains("HR") == true && permissionClaims?.Contains("leave_request.hr_management_leave_request") == true;

            var parameters = new DynamicParameters();

            parameters.Add("@OrgPositionId", request.OrgPositionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@IsHR", isHR, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@SidebarCount", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@AssignedCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var resultsFromDb = await _context.Database.GetDbConnection()
                .QueryAsync<object>(
                    "dbo.Approval_GET_CountWaitApprovalAndAssignInSideBar",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            return new CountWaitApprovalAndAssignedInSidebarResponse
            {
                TotalWaitApproval = parameters.Get<int>("@SidebarCount"),
                TotalAssigned = parameters.Get<int>("@AssignedCount")
            };
        }

        public async Task<PagedResults<HistoryApprovalResponse>> ListHistoryApprovedOrProcessed(ListHistoryApprovalRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@RequestTypeId", request.RequestTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@StatusId", request.Status, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var results = await _context.Database.GetDbConnection()
                .QueryAsync<HistoryApprovalResponse>(
                    "dbo.Approval_GET_GetHistoryApproval",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<HistoryApprovalResponse>
            {
                Data = (List<HistoryApprovalResponse>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }

        public async Task<PagedResults<PendingApproval>> ListAssigned(ListAssignedTaskRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Page", request.Page, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@PageSize", request.PageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@UserCode", request.UserCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@TotalRecords", dbType: DbType.Int32, direction: ParameterDirection.Output);


            var results = await _context.Database.GetDbConnection()
                .QueryAsync<PendingApproval>(
                    "dbo.Approval_GET_GetListAssigned",
                    parameters,
                    commandType: CommandType.StoredProcedure
            );

            int totalRecords = parameters.Get<int>("@TotalRecords");
            int totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PagedResults<PendingApproval>
            {
                Data = (List<PendingApproval>)results,
                TotalItems = totalRecords,
                TotalPages = totalPages
            };
        }
    }
}
