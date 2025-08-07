using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.Extensions;

namespace Repositories
{
    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateReport(Report report)
        {
            CreateReport(report);
        }

        public void DeleteReport(Report report)
        {
            DeleteReport(report);
        }

        public async Task<IEnumerable<ReportDto>> GetAllReportsAsync(ReportRequestParameters p)
        {
            var reports = await FindAll(false)
                .Include(r => r.Work)
                .Include(r => r.Account)
                    .ThenInclude(a => a!.Section)
                        .ThenInclude(s => s!.Department)
                .FilteredByDepartmentId(p.DepartmentId, r => r.Account!.Section!.DepartmentId)
                .FilteredBySearchTerm(p.SearchTerm ?? "", r => r.Account!.FirstName!)
                .FilteredByDate(p.StartDate ?? "", p.EndDate ?? "", r => r.CreatedAt)
                .FilteredByStatus(p.Status ?? "", r => r.Status!)
                .FilteredByType(p.Type ?? "")
                .SortExtensionForReports(p.SortBy ?? "")
                .ToPaginate(p.PageNumber, p.PageSize)
                .Select(r => new ReportDto()
                {
                    ReportId = r.ReportId,
                    ReportTitle = r.ReportTitle,
                    DepartmentName = r.Account!.Section!.Department!.DepartmentName,
                    SectionName = r.Account.Section.SectionName,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt,
                    AccountFirstName = r.Account!.FirstName,
                    AccountLastName = r.Account.LastName,
                    WorkName = r.Work != null ? r.Work.WorkName : ""
                })
                .ToListAsync();

            return reports;
        }

        public async Task<IEnumerable<Stats>> GetReportsStatusStatsAsync()
        {
            DateTime now = DateTime.UtcNow;
            DateTime lastMonth = DateTime.UtcNow.AddDays(-30);
            DateTime lastMonth2 = DateTime.UtcNow.AddDays(-60);

            var stats = await FindAll(false)
                .GroupBy(r => r.Status)
                .Select(g => new Stats()
                {
                    Key = g.Key,
                    TotalCount = g.Count(),
                    ThisMonthsCount = g.Count(r => r.CreatedAt >= lastMonth && r.CreatedAt <= now),
                    LastMonthsCount = g.Count(r => r.CreatedAt <= lastMonth && r.CreatedAt >= lastMonth2)
                })
                .ToListAsync();

            return stats;
        }

        public async Task<int> GetReportsCountAsync(ReportRequestParameters p)
        {
            var count = await FindAll(false)
                .FilteredByDepartmentId(p.DepartmentId, r => r.Account!.Section!.DepartmentId)
                .FilteredBySearchTerm(p.SearchTerm ?? "", r => r.Account!.FirstName!)
                .FilteredByDate(p.StartDate ?? "", p.EndDate ?? "", r => r.CreatedAt)
                .FilteredByStatus(p.Status ?? "", r => r.Status!)
                .FilteredByType(p.Type ?? "")
                .CountAsync();

            return count;
        }

        public async Task<int> GetAllReportsCountAsync()
        {
            var count = await FindAll(false)
                .CountAsync();

            return count;
        }

        public async Task<int> GetAllReportsCountOfOneUserAsync(string userId)
        {
            var count = await FindAllByCondition(r => r.AccountId == userId, false)
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<Report?>> GetAllReportsOfOneUserAsync(string userId)
        {
            var reports = await FindAllByCondition(r => r.AccountId == userId, false)
                .ToListAsync();

            return reports;
        }

        public async Task<IEnumerable<Report>> GetAllReportsOfOneWorkAsync(int workId)
        {
            var reports = await FindAllByCondition(r => r.WorkId == workId, false)
                .ToListAsync();

            return reports;
        }

        public Task<Report?> GetReportByIdAsync(int reportId)
        {
            var reports = FindByCondition(r => r.ReportId == reportId, false)
                .FirstOrDefaultAsync();

            return reports;
        }

        public void UpdateReport(Report report)
        {
            UpdateReport(report);
        }
    }
}
