using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;
using System.Threading.Tasks;

namespace Services
{
    public class ReportManager : IReportService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public ReportManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> CreateReportAsync(ReportDtoForCreation reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);
            _manager.Report.Create(report);
            await _manager.SaveAsync();

            _cache.Remove("reportsStats");
            _cache.Remove("reportsCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Rapor başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Reports"
            };
            return result;
        }

        public async Task<ResultDto> DeleteReportAsync(int reportId)
        {
            var report = await GetReportByIdForServiceAsync(reportId);
            _manager.Report.DeleteReport(report!);
            await _manager.SaveAsync();

            _cache.Remove("reportsStats");
            _cache.Remove("reportsCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Rapor başarıyla silindi",
                ResultType = "success",
                LoadComponent = "Reports"
            };
            return result;
        }

        public async Task<ResultDto> DeleteReportForUserAsync(int reportId, string userId)
        {
            var report = await GetReportByIdForServiceAsync(reportId);
            if (report?.AccountId == userId) 
            {
                _manager.Report.DeleteReport(report!);
                await _manager.SaveAsync();

                _cache.Remove("reportsStats");
                _cache.Remove("reportsCount");

                var result = new ResultDto()
                {
                    Success = true,
                    Message = "Raporunuz başarıyla silindi",
                    ResultType = "success",
                    LoadComponent = "Reports"
                };
                return result;
            }
            else
            {
                var result = new ResultDto()
                {
                    Success = false,
                    Message = "Rapor silinirken hata oluştu",
                    ResultType = "danger",
                    LoadComponent = "Reports"
                };
                return result;
            }
        }

        public async Task<IEnumerable<ReportDto?>> GetAllReportsAsync(ReportRequestParameters p)
        {
            var reportsDto = await _manager.Report.GetAllReportsAsync(p);

            return reportsDto;
        }

        public async Task<StatsDto> GetUserReportsStatsAsync(string userId)
        {
            var stats = await _manager.Report.GetUserReportsStatsAsync(userId);
            var statsDto = _mapper.Map<StatsDto>(stats);
            return statsDto;
        }

        public async Task<IEnumerable<StatsDto>> GetReportsStatusStatsAsync()
        {
            string cacheKey = "reportsStats";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<StatsDto>? cachedData))
            {
                return cachedData!;
            }

            var stats = await _manager.Report.GetReportsStatusStatsAsync();
            var statsDto = _mapper.Map<IEnumerable<StatsDto>>(stats);

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, statsDto, cacheOptions);
            return statsDto;
        }

        public async Task<int> GetAllReportsCountAsync()
        {
            var count = await _manager.Report.GetAllReportsCountAsync();
            return count;
        }

        public async Task<string> GetReportsCountForSidebarAsync()
        {
            string cacheKey = "reportsCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData.ToString() ?? "0";
            }

            var count = await _manager.Report.GetAllReportsCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count.ToString();
        }

        public async Task<int> GetReportsCountAsync(ReportRequestParameters p)
        {
            var count = await _manager.Report.GetReportsCountAsync(p);
            return count;
        }

        public async Task<int> GetAllReportsCountOfOneUserAsync(ReportRequestParameters p, string userId)
        {
            var count = await _manager.Report.GetAllReportsCountOfOneUserAsync(p, userId);
            return count;
        }

        public async Task<IEnumerable<ReportDto?>> GetAllReportsOfOneUserAsync(ReportRequestParameters p, string userId)
        {
            var reports = await _manager.Report.GetAllReportsOfOneUserAsync(p, userId);
            var reportsDto = _mapper.Map<IEnumerable<ReportDto>>(reports);

            return reportsDto;
        }

        public async Task<IEnumerable<ReportDto?>> GetAllReportsOfOneWorkAsync(int workId)
        {
            var reports = await _manager.Report.GetAllReportsOfOneWorkAsync(workId);
            var reportsDto = _mapper.Map<IEnumerable<ReportDto>>(reports);

            return reportsDto;
        }

        public async Task<ReportDto?> GetReportByIdAsync(int reportId)
        {
            var report = await _manager.Report.GetReportByIdAsync(reportId);
            if (report == null)
            {
                throw new KeyNotFoundException($"{report} id'sine sahip rapor bulunamadı.");
            }
            var reportDto = _mapper.Map<ReportDto>(report);
            return reportDto;
        }

        public async Task<ReportDtoForUpdate?> GetReportByIdForUpdateAsync(int reportId)
        {
            var report = await _manager.Report.GetReportByIdForUpdateAsync(reportId);
            if (report == null)
            {
                throw new KeyNotFoundException($"{report} id'sine sahip rapor bulunamadı.");
            }
            var reportDto = _mapper.Map<ReportDtoForUpdate>(report);
            return reportDto;
        }

        public async Task<ReportViewDto?> GetReportByIdForViewAsync(int reportId)
        {
            var report = await _manager.Report.GetReportByIdForViewAsync(reportId);
            if (report == null)
            {
                throw new KeyNotFoundException($"{report} id'sine sahip rapor bulunamadı.");
            }
            return report;
        }

        private async Task<Report?> GetReportByIdForServiceAsync(int reportId)
        {
            var report = await _manager.Report.GetReportByIdAsync(reportId);
            if (report == null)
            {
                throw new KeyNotFoundException($"{report} id'sine sahip rapor bulunamadı.");
            }
            return report;
        }

        public async Task<Dictionary<string, int>> GetDailyReportsCountOfOneUser(string userId)
        {
            var dailyReportsCount = await _manager.Report.GetDailyReportsCountOfOneUser(userId);

            return dailyReportsCount;
        }

        public async Task<ResultDto> UpdateReportAsync(ReportDtoForUpdate reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);

            var intern = new Account { Id = reportDto.AccountId! };
            _manager.Account.Attach(intern);

            _manager.Report.UpdateReport(report);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Rapor başarıyla güncellendi",
                ResultType = "success",
                LoadComponent = "Reports"
            };
            return result;

        }

        public async Task<ResultDto> ChangeStatusAsync(int reportId)
        {
            var work = await _manager.Report.GetReportByIdForUpdateAsync(reportId);

            if (work == null)
                throw new KeyNotFoundException($"{reportId} id'sine sahip görev bulunamadı.");


            work.Status = ReportStatus.Read;
            _manager.Report.UpdateReport(work);
            await _manager.SaveAsync();

            _cache.Remove("reportsStats");

            return new ResultDto
            {
                Success = true,
                Message = "Rapor durumu başarıyla güncellendi.",
                ResultType = "success",
            };
        }

        public async Task<string> GetAllReportsCountOfOneUserForSidebarAsync(string userId)
        {
            var count = await _manager.Report.GetAllReportsCountOfOneUserForSidebarAsync(userId);

            return count.ToString();
        }
    }
}
