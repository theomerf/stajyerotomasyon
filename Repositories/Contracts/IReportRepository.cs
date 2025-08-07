using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;

namespace Repositories.Contracts
{
    public interface IReportRepository : IRepositoryBase<Report>
    {
        Task<IEnumerable<ReportDto>> GetAllReportsAsync(ReportRequestParameters p);
        Task<Report?> GetReportByIdAsync(int reportId);
        Task<IEnumerable<Report?>> GetAllReportsOfOneUserAsync(string userId);
        Task<IEnumerable<Report>> GetAllReportsOfOneWorkAsync(int workId);
        Task<IEnumerable<Stats>> GetReportsStatusStatsAsync();
        Task<int> GetAllReportsCountAsync();
        Task<int> GetReportsCountAsync(ReportRequestParameters p);
        Task<int> GetAllReportsCountOfOneUserAsync(string userId);
        void CreateReport(Report report);
        void DeleteReport(Report report);
        void UpdateReport(Report report);
    }
}
