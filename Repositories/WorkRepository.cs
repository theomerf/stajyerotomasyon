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
                    Status = w.Status.ToString(),
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

        public async Task<Stats> GetUserWorksStatsAsync(string userId)
        {
            var stats = await FindAllByCondition(w => w.Interns!.Any(i => i.Id == userId) && w.Status == WorkStatus.Active, false)
                .GroupBy(_ => 1)
                .Select(g => new Stats()
                {
                    Key = g.Key.ToString(),
                    TotalCount = g.Count(),
                    ThisMonthsCount = 0,
                    LastMonthsCount = 0,
                })
                .FirstOrDefaultAsync();

            return stats ?? new Stats();
        }

        public async Task<int> GetAllWorksCountOfOneUserAsync(string userId)
        {
            var count = await FindAllByCondition(w => w.Interns!.Any(i => i.Id == userId), false)
                .CountAsync();

            return count;
        }

        public async Task<IEnumerable<WorkDto?>> GetAllWorksOfOneUserAsync(string userId)
        {
            var works = await FindAllByCondition(w => w.Interns!.Any(i => i.Id == userId), false)
                .Include(w => w.TaskMaster!)
                .Include(w => w.Reports)
                .Include(w => w.Interns!)
                .OrderByDescending(w => w.Status == WorkStatus.Active)
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
                     Status = w.Status.ToString(),
                })
                .ToListAsync();

            return works;
        }

        public async Task<IEnumerable<WorkDtoForReports?>> GetAllWorkNamesOfOneUserAsync(string userId)
        {
            var works = await FindAllByCondition(w => w.Interns!.Any(i => i.Id == userId) && w.Status == WorkStatus.Active, false)
                .Include(w => w.TaskMaster!)
                .Include(w => w.Reports)
                .Include(w => w.Interns!)
                .Select(w => new WorkDtoForReports()
                {
                    WorkId = w.WorkId,
                    WorkName = w.WorkName,
                    WorkStartDate = w.WorkStartDate!.Value,
                    WorkEndDate = w.WorkEndDate!.Value,
                })
                .ToListAsync();

            return works;
        }

        public async Task<WorkViewDto?> GetWorkByIdForViewAsync(int workId)
        {
            var work = await FindByCondition(w => w.WorkId == workId, false)
                .Include(w => w.TaskMaster!)
                .Include(w => w.Reports!)
                .ThenInclude(r => r.Account)
                .Include(w => w.Interns!)
                .Select(w => new WorkViewDto()
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
                    Status = w.Status.ToString(),
                    Reports = w.Reports,
                    InternsName = w.Interns!
                        .Select(i => i.FirstName + " " + i.LastName)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return work;
        }

        public async Task<WorkViewDto?> GetWorkByIdForViewOfOneUserAsync(int workId, string userId)
        {
            var work = await FindByCondition(w => w.WorkId == workId, false)
                .Include(w => w.TaskMaster!)
                .Include(w => w.Reports!)
                .ThenInclude(r => r.Account)
                .Include(w => w.Interns!)
                .Select(w => new WorkViewDto()
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
                    Status = w.Status.ToString(),
                    Reports = w.Reports!.Where(r => r.AccountId == userId).ToList(),
                    InternsName = w.Interns!
                        .Select(i => i.FirstName + " " + i.LastName)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            return work;
        }

        public async Task<Work?> GetWorkByIdAsync(int workId)
        {
            var work = await FindByCondition(w => w.WorkId == workId, false)
                .FirstOrDefaultAsync();

            return work;
        }

        public async Task<Work?> GetWorkForUpdateByIdAsync(int workId)
        {
            var work = await FindByCondition(w => w.WorkId == workId, true)
                .Include(w => w.Interns!)
                .FirstOrDefaultAsync();

            return work;
        }

        public void UpdateWork(Work work)
        {
            Update(work);
        }
    }
}
