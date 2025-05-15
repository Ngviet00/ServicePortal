using Microsoft.EntityFrameworkCore;
using ServicePortal.Domain.Entities;
using ServicePortal.Infrastructure.Data;

namespace ServicePortal.Modules.User.Services
{
    public class OrgChartBuilder
    {
        private readonly ApplicationDbContext _context;

        public OrgChartBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrgChartNode> BuildTree(int? departmentId)
        {
            var rules = await _context.ApprovalFlows.Where(r => r.DepartmentId == departmentId).ToListAsync();

            var positionIds = rules.SelectMany(r => new[] { r.FromPosition, r.ToPosition }).Distinct().ToList();

            var people = await _context.Users
                .Where(u => positionIds.Contains(u.PositionId))
                .Select(u => new Person
                {
                    Usercode = u.UserCode,
                    PositionId = u.PositionId ?? -0
                })
                .ToListAsync();

            var tree = BuildOrgChart(departmentId ?? 0, people, rules);

            return tree;
        }

        private OrgChartNode BuildOrgChart(int departmentId, List<Person> people, List<ApprovalFlow> rules)
        {
            var lastStepRule = rules
                .Where(r => r.DepartmentId == departmentId)
                .OrderByDescending(r => r.StepOrder)
                .FirstOrDefault();

            if (lastStepRule == null || lastStepRule.ToPosition == null)
            {
                return new OrgChartNode
                {
                    PositionId = 0,
                    People = new List<Person>(),
                    Children = new List<OrgChartNode>()
                };
            }

            var rootPositionId = lastStepRule.ToPosition.Value;

            var personLookup = people
                .GroupBy(p => p.PositionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var ruleLookup = rules
                .Where(r => r.DepartmentId == departmentId && r.FromPosition != null && r.ToPosition != null)
                .GroupBy(r => r.ToPosition.Value)
                .ToDictionary(g => g.Key, g => g.Select(r => r.FromPosition.Value).ToList());

            OrgChartNode BuildNode(int positionId)
            {
                return new OrgChartNode
                {
                    PositionId = positionId,
                    People = personLookup.ContainsKey(positionId) ? personLookup[positionId] : new List<Person>(),
                    Children = ruleLookup.ContainsKey(positionId)
                        ? ruleLookup[positionId].Select(pid => BuildNode(pid)).ToList()
                        : new List<OrgChartNode>()
                };
            }

            return BuildNode(rootPositionId);
        }
    }
    public class Person
    {
        public string Usercode { get; set; }
        public int PositionId { get; set; }
    }
    public class OrgChartNode
    {
        public int PositionId { get; set; }
        public List<Person> People { get; set; } = new();
        public List<OrgChartNode> Children { get; set; } = new();
    }
}
