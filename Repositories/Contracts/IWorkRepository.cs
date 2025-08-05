using Entities.Models;

namespace Repositories.Contracts
{
    public interface IWorkRepository : IRepositoryBase<Work>
    {
        Task<IEnumerable<Work>> GetAllWorksAsync();
        Task<Work?> GetWorkByIdAsync(int workId);
        Task<IEnumerable<Work?>> GetAllWorksOfOneUserAsync(string userId);
        Task<int> GetAllWorksCountAsync();
        Task<int> GetAllWorksCountOfOneUserAsync(string userId);
        void CreateWork(Work work);
        void DeleteWork(Work work);
        void UpdateWork(Work work);
    }
}
