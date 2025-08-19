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
        Task<Dictionary<string, int>> GetDailyReportsCountOfOneUser(string userId);
        Task<StatsDto> GetUserReportsStatsAsync(string userId);
        Task<ResultDto> ChangeStatusAsync(int reportId);
        Task<IEnumerable<ReportDto?>> GetAllReportsOfOneWorkAsync(int workId);
        Task<int> GetAllReportsCountAsync();
        Task<ReportDtoForUpdate?> GetReportByIdForUpdateAsync(int reportId);
        Task<int> GetReportsCountAsync(ReportRequestParameters p);
        Task<int> GetAllReportsCountOfOneUserAsync(ReportRequestParameters p, string userId);
        Task<ResultDto> CreateReportAsync(ReportDtoForCreation reportDto);
        Task<ResultDto> DeleteReportAsync(int reportId);
        Task<ResultDto> UpdateReportAsync(ReportDtoForUpdate reportDto);
        Task<ReportViewDto?> GetReportByIdForViewAsync(int reportId);
    }
}
