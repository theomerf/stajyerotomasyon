using Entities.Dtos;
using Entities.RequestParameters;

namespace Services.Contracts
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto?>> GetAllReportsAsync(ReportRequestParameters p);
        Task<ReportDto?> GetReportByIdAsync(int reportId);
        Task<IEnumerable<ReportDto?>> GetAllReportsOfOneUserAsync(ReportRequestParameters p, string userId);
        Task<IEnumerable<StatsDto>> GetReportsStatusStatsAsync();
        Task<string> GetReportsCountForSidebarAsync();
        Task<ResultDto> DeleteReportForUserAsync(int reportId, string userId);
        Task<string> GetAllReportsCountOfOneUserForSidebarAsync(string userId);
        Task<ResultDto> ChangeStatusAsync(int reportId);
        Task<IEnumerable<ReportDto?>> GetAllReportsOfOneWorkAsync(int workId);
        Task<int> GetAllReportsCountAsync();
        Task<int> GetReportsCountAsync(ReportRequestParameters p);
        Task<int> GetAllReportsCountOfOneUserAsync(ReportRequestParameters p, string userId);
        Task<ResultDto> CreateReportAsync(ReportDto reportDto);
        Task<ResultDto> DeleteReportAsync(int reportId);
        Task<ResultDto> UpdateReportAsync(ReportDto reportDto);
        Task<ReportViewDto?> GetReportByIdForViewAsync(int reportId);
    }
}
