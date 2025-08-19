using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;

namespace Repositories.Contracts
{
    public interface IWorkRepository : IRepositoryBase<Work>
    {
        Task<IEnumerable<WorkDto>> GetAllWorksAsync(WorkRequestParameters p);
        Task<Work?> GetWorkByIdAsync(int workId);
        Task<Work?> GetWorkForUpdateByIdAsync(int workId);
        Task<WorkViewDto?> GetWorkByIdForViewAsync(int workId);
        Task<Stats> GetUserWorksStatsAsync(string userId);
        Task<IEnumerable<WorkDto?>> GetAllWorksOfOneUserAsync(string userId);
        Task<WorkViewDto?> GetWorkByIdForViewOfOneUserAsync(int workId, string userId);
        Task<IEnumerable<WorkDtoForReports?>> GetAllWorkNamesOfOneUserAsync(string userId);
        Task<int> GetAllWorksCountAsync();
        Task<int> GetWorksCountAsync(WorkRequestParameters p);
        Task<int> GetAllWorksCountOfOneUserAsync(string userId);
        void CreateWork(Work work);
        void DeleteWork(Work work);
        void UpdateWork(Work work);
    }
}
