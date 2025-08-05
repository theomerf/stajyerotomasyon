using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using System;

namespace Repositories.Contracts
{
    public interface IApplicationRepository : IRepositoryBase<Application>
    {
        Task<IEnumerable<Application>> GetAllApplicationsAsync(ApplicationRequestParameters p);
        Task<int> GetAllApplicationsCountAsync();
        Task<int> GetApplicationsCountAsync(ApplicationRequestParameters p);
        Task<Stats> GetApplicationsOnWaitStatsAsync();
        Task<List<Note>> GetAllNotesForOneApplicationAsync(int applicationId);
        Task<Dictionary<string, int>> GetMonthlyApplicationCountsAsync();
        Task<IEnumerable<Stats>> GetApplicationsStatusStatsAsync();
        Task<Application?> GetApplicationByIdAsync(int applicationId);
        void DeleteApplication(Application application);
        void UpdateApplication(Application application);
        void CreateApplication(Application application);
    }
}
