using Entities.Dtos;
using Entities.RequestParameters;

namespace Services.Contracts
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto?>> GetAllReportsAsync(ReportRequestParameters p);
        Task<ReportDto?> GetReportByIdAsync(int reportId);
        Task<IEnumerable<ReportDto?>> GetAllReportsOfOneUserAsync(string userId);
        Task<IEnumerable<StatsDto>> GetReportsStatusStatsAsync();
        Task<IEnumerable<ReportDto?>> GetAllReportsOfOneWorkAsync(int workId);
        Task<int> GetAllReportsCountAsync();
        Task<int> GetReportsCountAsync(ReportRequestParameters p);
        Task<int> GetAllReportsCountOfOneUser(string userId);
        Task<ResultDto> CreateReportAsync(ReportDto reportDto);
        Task<ResultDto> DeleteReportAsync(int reportId);
        Task<ResultDto> UpdateReportAsync(ReportDto reportDto);
    }
}
