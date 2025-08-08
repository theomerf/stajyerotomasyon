using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ApplicationRepository : RepositoryBase<Application>, IApplicationRepository
    {
        public ApplicationRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateApplication(Application application)
        {
            Create(application);
        }

        public void DeleteApplication(Application application)
        {
            Remove(application);
        }

        public void UpdateApplication(Application application)
        {
            Update(application);
        }

        public async Task<IEnumerable<Application>> GetAllApplicationsAsync(ApplicationRequestParameters p)
        {
            var applications = await FindAll(false)
                .Include(a => a.Section)
                    .ThenInclude(s => s!.Department)
                .FilteredByDate(p.StartDate ?? "", p.EndDate ?? "", a => a.CreatedDate)
                .FilteredByStatus(p.Status ?? "", a => a.Status.ToString()!)
                .FilteredBySearchTerm(p.SearchTerm ?? "", a => a.ApplicantFirstName!)
                .FilteredByDepartmentId(p.DepartmentId, a => a.Section!.DepartmentId)
                .SortExtensionForApplications(p.SortBy ?? "")
                .ToPaginate(p.PageNumber, p.PageSize)
                .ToListAsync();

            return applications;
        }

        public async Task<int> GetApplicationsCountAsync(ApplicationRequestParameters p)
        {
            var count = await FindAll(false)
                .FilteredByDate(p.StartDate ?? "", p.EndDate ?? "", a => a.CreatedDate)
                .FilteredByStatus(p.Status ?? "", a => a.Status.ToString()!)
                .FilteredBySearchTerm(p.SearchTerm ?? "", a => a.ApplicantFirstName!)
                .FilteredByDepartmentId(p.DepartmentId, a => a.Section!.DepartmentId)
                .CountAsync();

            return count;
        }
        public async Task<List<Note>> GetAllNotesForOneApplicationAsync(int applicationId)
        {
            return await _context.Notes
                .Where(n => n.ApplicationId == applicationId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }


        public async Task<int> GetAllApplicationsCountAsync()
        {
            var count = await FindAll(false)
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<Stats>> GetApplicationsStatusStatsAsync()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastMonth = DateTime.UtcNow.AddDays(-30);
            DateTime lastMonth2 = DateTime.UtcNow.AddDays(-60);

            var stats = await FindAll(false)
                .GroupBy(a => a.Status)
                .Select(g => new Stats()
                {
                    Key = g.Key.ToString(),
                    TotalCount = g.Count(),
                    ThisMonthsCount = g.Count(a => a.UpdatedDate >= lastMonth && a.UpdatedDate <= now),
                    LastMonthsCount = g.Count(a => a.UpdatedDate <= lastMonth && a.UpdatedDate >= lastMonth2)
                })
                .ToListAsync();

            var allStatuses = Enum.GetValues(typeof(ApplicationStatus))
            .Cast<ApplicationStatus>();

            var finalStats = allStatuses
                .Select(status => stats.FirstOrDefault(s => s.Key == status.ToString())
            ?? new Stats
            {
                Key = status.ToString(),
                TotalCount = 0,
                ThisMonthsCount = 0,
                LastMonthsCount = 0
            })
            .ToList();

            return finalStats;
        }

        public async Task<Stats> GetApplicationsOnWaitStatsAsync()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastMonth = DateTime.UtcNow.AddDays(-30);
            DateTime lastMonth2 = DateTime.UtcNow.AddDays(-60);

            var stats = await FindAllByCondition(a => a.Status.ToString() == "OnWait", false)
                .GroupBy(_ => 1)
                .Select(g => new Stats()
                {
                    TotalCount = g.Count(),
                    ThisMonthsCount = g.Count(a => a.UpdatedDate >= lastMonth && a.UpdatedDate <= now),
                    LastMonthsCount = g.Count(a => a.UpdatedDate <= lastMonth && a.UpdatedDate >= lastMonth2)
                })
                .FirstOrDefaultAsync();

            return stats ?? new Stats();
        }

        public async Task<Application?> GetApplicationByIdAsync(int applicationId)
        {
            var application = await FindByCondition(a => a.ApplicationId == applicationId, true)
                .AsQueryable()
                .Include(a => a.Notes)
                .Include(a => a!.Section)
                    .ThenInclude(s => s!.Department)!
                .FirstOrDefaultAsync();

            return application;
        }

        public async Task<Dictionary<string, int>> GetMonthlyApplicationCountsAsync()
        {
            var startOfYear = new DateTime(DateTime.UtcNow.Year, 1, 1);
            var now = DateTime.UtcNow;

            var rawData = await FindAll(false)
                .Where(u => u.CreatedDate >= startOfYear && u.CreatedDate <= now)
                .GroupBy(u => new { u.CreatedDate.Year, u.CreatedDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .ToListAsync();

            var culture = new CultureInfo("tr-TR");
            var result = new Dictionary<string, int>();

            for (int month = 1; month <= now.Month; month++)
            {
                string monthName = culture.DateTimeFormat.GetMonthName(month);
                int count = rawData.FirstOrDefault(x => x.Month == month)?.Count ?? 0;
                result[monthName] = count;
            }

            return result;
        }


    }
}
