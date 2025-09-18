using Microsoft.EntityFrameworkCore;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Extensions
{
    public static class ApplicationFormExtensions
    {
        public static async Task<ApplicationForm?> LoadApplicationForm(this IQueryable<ApplicationForm> query, DbContext context, Guid? applicationFormId)
        {
            if (applicationFormId == null)
            {
                return null;
            }

            var applicationForm = await query
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .Include(e => e.AssignedTasks)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (applicationForm != null)
            {
                applicationForm.HistoryApplicationForms = await context.Set<HistoryApplicationForm>()
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .ToListAsync();
            }

            return applicationForm;
        }
    }
}
