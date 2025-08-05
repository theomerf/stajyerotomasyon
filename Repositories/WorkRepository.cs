using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;

namespace Repositories
{
    public class WorkRepository : RepositoryBase<Work>, IWorkRepository
    {
        public WorkRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateWork(Work work)
        {
            CreateWork(work);
        }

        public void DeleteWork(Work work)
        {
            DeleteWork(work);
        }

        public async Task<IEnumerable<Work>> GetAllWorksAsync()
        {
            var works = await FindAll(false)
                .ToListAsync();

            return works;
        }

        public async Task<int> GetAllWorksCountAsync()
        {
            var count = await FindAll(false)
                .CountAsync();

            return count;
        }

        public async Task<int> GetAllWorksCountOfOneUserAsync(string userId)
        {
            var count = await FindAllByCondition(w => w.TaskMasterId == userId, false)
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<Work?>> GetAllWorksOfOneUserAsync(string userId)
        {
            var works = await FindAllByCondition(w => w.TaskMasterId == userId, false)
                .ToListAsync();

            return works;
        }

        public async Task<Work?> GetWorkByIdAsync(int workId)
        {
            var work = await FindByCondition(w => w.WorkId == workId, false)
                .FirstOrDefaultAsync();

            return work;
        }

        public void UpdateWork(Work work)
        {
            UpdateWork(work);
        }
    }
}
