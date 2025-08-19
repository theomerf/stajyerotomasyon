using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;

namespace Repositories.Contracts
{
    public interface IReportRepository : IRepositoryBase<Report>
    {
        Task<IEnumerable<ReportDto>> GetAllReportsAsync(ReportRequestParameters p);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task<IEnumerable<ReportDto>> GetAllReportsOfOneUserAsync(ReportRequestParameters p, string userId);
        Task<IEnumerable<Report>> GetAllReportsOfOneWorkAsync(int workId);
        Task<int> GetAllReportsCountOfOneUserForSidebarAsync(string userId);
        Task<IEnumerable<Stats>> GetReportsStatusStatsAsync();
        Task<Stats> GetUserReportsStatsAsync(string userId);
        Task<Dictionary<String, int>> GetDailyReportsCountOfOneUser(string userId);
        Task<Report?> GetReportByIdForUpdateAsync(int reportId);
        Task<int> GetAllReportsCountAsync();
        Task<int> GetReportsCountAsync(ReportRequestParameters p);
        Task<int> GetAllReportsCountOfOneUserAsync(ReportRequestParameters p, string userId);
        Task<ReportViewDto?> GetReportByIdForViewAsync(int reportId);
        void CreateReport(Report report);
        void DeleteReport(Report report);
        void UpdateReport(Report report);
    }
}
