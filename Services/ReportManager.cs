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

        public async Task<ResultDto> CreateReportAsync(ReportDto reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);
            _manager.Report.Create(report);
            await _manager.SaveAsync();

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

            var result = new ResultDto()
            {
                Success = true,
                Message = "Rapor başarıyla silindi",
                ResultType = "success",
                LoadComponent = "Reports"
            };
            return result;
        }

        public async Task<IEnumerable<ReportDto?>> GetAllReportsAsync(ReportRequestParameters p)
        {
            var reportsDto = await _manager.Report.GetAllReportsAsync(p);

            return reportsDto;
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

        public Task<int> GetAllReportsCountAsync()
        {
            var count = _manager.Report.GetAllReportsCountAsync();
            return count;
        }

        public Task<int> GetReportsCountAsync(ReportRequestParameters p)
        {
            var count = _manager.Report.GetReportsCountAsync(p);
            return count;
        }

        public async Task<int> GetAllReportsCountOfOneUser(string userId)
        {
            var count = await _manager.Report.GetAllReportsCountOfOneUserAsync(userId);
            return count;
        }

        public async Task<IEnumerable<ReportDto?>> GetAllReportsOfOneUserAsync(string userId)
        {
            var reports = await _manager.Report.GetAllReportsOfOneUserAsync(userId);
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

        private Task<Report?> GetReportByIdForServiceAsync(int reportId)
        {
            var report = _manager.Report.GetReportByIdAsync(reportId);
            if (report == null)
            {
                throw new KeyNotFoundException($"{report} id'sine sahip rapor bulunamadı.");
            }
            return report;
        }

        public async Task<ResultDto> UpdateReportAsync(ReportDto reportDto)
        {
            var report = _mapper.Map<Report>(reportDto);

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
    }
}
