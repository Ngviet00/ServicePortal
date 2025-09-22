using Microsoft.EntityFrameworkCore;
using ServicePortals.Domain.Entities;

namespace ServicePortals.Application.Extensions
{
    public static class ApplicationFormExtensions
    {
        public static async Task<ApplicationForm?> LoadApplicationForm(this IQueryable<ApplicationForm> query, DbContext context, long applicationFormId)
        {
            if (applicationFormId == 0)
            {
                return null;
            }

            var applicationForm = await query
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .Include(e => e.AssignedTasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == applicationFormId);

            if (applicationForm != null)
            {
                applicationForm.HistoryApplicationForms = await context.Set<HistoryApplicationForm>()
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return applicationForm;
        }

        public static async Task<ApplicationForm?> LoadApplicationForm(this IQueryable<ApplicationForm> query, DbContext context, string applicationFormCode)
        {
            if (string.IsNullOrWhiteSpace(applicationFormCode))
            {
                return null;
            }

            var applicationForm = await query
                .Include(e => e.RequestType)
                .Include(e => e.RequestStatus)
                .Include(e => e.OrgUnit)
                .Include(e => e.AssignedTasks)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Code == applicationFormCode);

            if (applicationForm != null)
            {
                applicationForm.HistoryApplicationForms = await context.Set<HistoryApplicationForm>()
                    .Where(e => e.ApplicationFormId == applicationForm.Id)
                    .OrderByDescending(e => e.ActionAt)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return applicationForm;
        }
    }
}
