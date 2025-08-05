using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

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

        public async Task<IEnumerable<Report>> GetAllReportsAsync(ReportRequestParameters p)
        {
            var reports = await FindAll(false)
                .Include(r => r.Work)
                .Include(r => r.Account)
                .ToListAsync();

            return reports;
        }

        public async Task<int> GetAllReportsCountAsync(ReportRequestParameters p)
        {
            var count = await FindAll(false)
                .Include(r => r.Work)
                .Include(r => r.Account)
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
