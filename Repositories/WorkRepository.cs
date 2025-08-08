using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
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
            Create(work);
        }

        public void DeleteWork(Work work)
        {
            Remove(work);
        }

        public async Task<IEnumerable<WorkDto>> GetAllWorksAsync(WorkRequestParameters p)
        {
            var reports = await FindAll(false)
                .Include(w => w.TaskMaster!)
                .Include(w => w.Reports)
                .Include(w => w.Interns!)
                .Select(w => new WorkDto()
                {
                    WorkId = w.WorkId,
                    WorkName = w.WorkName,
                    WorkDescription = w.WorkDescription,
                    ImageUrls = w.ImageUrls,
                    WorkStartDate = w.WorkStartDate!.Value,
                    WorkEndDate = w.WorkEndDate!.Value,
                    TaskMasterName = String.Concat(w.TaskMaster!.FirstName, " ", w.TaskMaster.LastName),
                    InternsCount = w.Interns!.Count(),
                    ReportsCount = w.Reports!.Count(),
                })
                .ToListAsync();

            return reports;
        }

        public async Task<int> GetAllWorksCountAsync()
        {
            var count = await FindAll(false)
                .CountAsync();

            return count;
        }

        public async Task<int> GetWorksCountAsync(WorkRequestParameters p)
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
            Update(work);
        }
    }
}
