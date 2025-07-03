using Microsoft.EntityFrameworkCore;
using ServicePortals.Application.Dtos.User.Requests;
using ServicePortals.Domain.Entities;
using ServicePortals.Infrastructure.Data;

namespace ServicePortals.Infrastructure.Services.User
{
    public class OrgChartService
    {
        private readonly ApplicationDbContext _context;

        //public OrgChartService(ApplicationDbContext context)
        //{
        //    _context = context;
        //}

        public async Task<OrgChartRequest> BuildTree(int? departmentId)
        {

            return null;
            //return await _orgChartBuilder.BuildTree(departmentId);
        }

        //public async Task<OrgChartRequest> BuildTree(int? departmentId)
        //{
        //    var rules = await _context.ApprovalFlows.Where(r => r.DepartmentId == departmentId).ToListAsync();

        //    var positionIds = rules.SelectMany(r => new[] { r.FromPosition, r.ToPosition }).Distinct().ToList();

        //    var people = await _context.Users
        //        .Where(u => positionIds.Contains(u.PositionId))
        //        .Select(u => new Person
        //        {
        //            Usercode = u.UserCode,
        //            PositionId = u.PositionId ?? -0
        //        })
        //        .ToListAsync();

        //    var tree = BuildOrgChart(departmentId ?? 0, people, rules);

        //    return tree;
        //}

        //private OrgChartRequest BuildOrgChart(int departmentId, List<Person> people, List<ApprovalFlow> rules)
        //{
        //    var lastStepRule = rules
        //        .Where(r => r.DepartmentId == departmentId)
        //        .OrderByDescending(r => r.StepOrder)
        //        .FirstOrDefault();

        //    if (lastStepRule == null || lastStepRule.ToPosition == null)
        //    {
        //        return new OrgChartRequest
        //        {
        //            PositionId = 0,
        //            People = new List<Person>(),
        //            Children = new List<OrgChartRequest>()
        //        };
        //    }

        //    var rootPositionId = lastStepRule.ToPosition.Value;

        //    var personLookup = people
        //        .Where(p => p.PositionId.HasValue)
        //        .GroupBy(p => p.PositionId!.Value)
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    var ruleLookup = rules
        //        .Where(r => r.DepartmentId == departmentId && r.FromPosition.HasValue && r.ToPosition.HasValue)
        //        .Select(r => new {
        //            From = r.FromPosition!.Value,
        //            To = r.ToPosition!.Value
        //        })
        //        .GroupBy(r => r.To)
        //        .ToDictionary(g => g.Key, g => g.Select(r => r.From).ToList());

        //    OrgChartRequest BuildNode(int positionId)
        //    {
        //        return new OrgChartRequest
        //        {
        //            PositionId = positionId,
        //            People = personLookup.ContainsKey(positionId) ? personLookup[positionId] : [],
        //            Children = ruleLookup.ContainsKey(positionId)
        //                ? ruleLookup[positionId].Select(pid => BuildNode(pid)).ToList()
        //                : new List<OrgChartRequest>()
        //        };
        //    }

        //    return BuildNode(rootPositionId);
        //}
    }
}
