using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDto>> GetAllApplicationsAsync(ApplicationRequestParameters p);
        Task<IEnumerable<StatsDto>> GetApplicationsStatusStatsAsync();
        Task<Dictionary<string, int>> GetMonthlyApplicationCountsAsync();
        Task<ApplicationDto> ChangeApplicationSeenAsync(int applicationId);
        Task<StatsDto> GetApplicationsOnWaitStatsAsync();
        Task ExportToInternsAsync(int applicationId);
        Task<List<Note>> GetAllNotesForOneApplicationAsync(int applicationId);
        Task<ResultDto> AddNoteToApplicationAsync(int applicationId, string note, string userId);
        Task<ResultDto> RemoveNoteFromApplicationAsync(int applicationId, int noteId);
        Task<int> GetAllApplicationsCountAsync();
        Task<AccountDtoForCreation> GetApplicationForExportAsync(int applicationId, string userNo);
        Task<ResultDto> ChangeApplicationStatusAsync(int applicationId, string status);
        Task<int> GetApplicationsCountAsync(ApplicationRequestParameters p);
        Task<ApplicationDto?> GetApplicationByIdAsync(int applicationId);
        Task<ResultDto> DeleteApplicationAsync(int applicationId);
/*       Task UpdateApplicationAsync(ApplicationDtoForUpdate applicationDto);
        Task CreateApplicationAsync(ApplicationDtoForCreation applicationDto);*/
    }
}
