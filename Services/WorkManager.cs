using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repositories;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class WorkManager : IWorkService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public WorkManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> CreateWorkAsync(WorkDtoForCreation workDto)
        {
            var work = _mapper.Map<Work>(workDto);
            if (workDto.BroadcastType == "All")
            {
                var internsId = await _manager.Account.GetAllInternsId();
                var interns = internsId.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                work.Interns = interns;
            }
            else if (workDto.BroadcastType == "Users")
            {
                var interns = workDto.InternsId!.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                work.Interns = interns;
            }
            else if (workDto.BroadcastType == "Department")
            {
                var internsId = await _manager.Account.GelAllInternsOfDepartment(workDto.DepartmentId!.Value);
                var interns = internsId.Select(id => new Account { Id = id }).ToList();
                _manager.Account.AttachRange(interns);
                work.Interns = interns;
            }
            else if (workDto.BroadcastType == "Section")
            {
                var internsId = await _manager.Account.GelAllInternsOfSection(workDto.SectionId!.Value);
                var interns = internsId.Select(id => new Account { Id = id! }).ToList();
                _manager.Account.AttachRange(interns);
                work.Interns = interns;
            }

            _manager.Work.CreateWork(work);
            await _manager.SaveAsync();

            _cache.Remove("worksCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Görev başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Works"
            };
            return result;
        }

        public async Task<ResultDto> DeleteWorkAsync(int workId)
        {
            var work = await GetWorkByIdForServiceAsync(workId);
            _manager.Work.DeleteWork(work!);
            await _manager.SaveAsync();

            _cache.Remove("worksCount");

            var result = new ResultDto()
            {
                Success = true,
                Message = "Görev başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Works"
            };
            return result;
        }

        public async Task<StatsDto> GetUserWorksStatsAsync(string userId)
        {
            var stats = await _manager.Work.GetUserWorksStatsAsync(userId);
            var statsDto = _mapper.Map<StatsDto>(stats);

            return statsDto;
        }

        public async Task<WorkViewDto?> GetWorkByIdForViewOfOneUserAsync(int workId, string userId) 
        {
            var work = await _manager.Work.GetWorkByIdForViewOfOneUserAsync(workId, userId);
            if (work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }
            return work;
        }
        public async Task<IEnumerable<WorkDto?>> GetAllWorksAsync(WorkRequestParameters p)
        {
            var works = await _manager.Work.GetAllWorksAsync(p);
            var worksDto = _mapper.Map<IEnumerable<WorkDto>>(works);

            return worksDto;
        }

        public async Task<int> GetAllWorksCountAsync()
        {
            var count = await _manager.Work.GetAllWorksCountAsync();
            return count;
        }

        public async Task<string> GetWorksCountForSidebarAsync()
        {
            string cacheKey = "worksCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData!.ToString() ?? "";
            }

            var count = await _manager.Work.GetAllWorksCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count.ToString();
        }

        public async Task<int> GetWorksCountAsync(WorkRequestParameters p)
        {
            var count = await _manager.Work.GetWorksCountAsync(p);
            return count;
        }

        public async Task<int> GetAllWorksCountOfOneUser(string userId)
        {
            var count = await _manager.Work.GetAllWorksCountOfOneUserAsync(userId);
            return count;
        }

        public async Task<IEnumerable<WorkDto?>> GetAllWorksOfOneUserAsync(string userId)
        {
            var works = await _manager.Work.GetAllWorksOfOneUserAsync(userId);
            var worksDto = _mapper.Map<IEnumerable<WorkDto>>(works);

            return worksDto;
        }

        public async Task<IEnumerable<WorkDtoForReports?>> GetAllWorkNamesOfOneUserAsync(string userId)
        {
            var works = await _manager.Work.GetAllWorkNamesOfOneUserAsync(userId);

            return works;
        }

        public async Task<WorkDto?> GetWorkByIdAsync(int workId)
        {
            var work = await _manager.Work.GetWorkByIdAsync(workId);
            if (work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }
            var workDto = _mapper.Map<WorkDto>(work);
            return workDto;
        }

        public async Task<Work?> GetWorkByIdForServiceAsync(int workId)
        {
            var work = await _manager.Work.GetWorkByIdAsync(workId);
            if (work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }
            return work;
        }

        public async Task<WorkViewDto?> GetWorkByIdForViewAsync(int workId)
        {
            var work = await _manager.Work.GetWorkByIdForViewAsync(workId);
            if (work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }
            return work;
        }

        public async Task<WorkDtoForUpdate?> GetWorkForUpdateByIdAsync(int workId)
        {
            var work = await _manager.Work.GetWorkForUpdateByIdAsync(workId);
            if (work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }

            var workDto = _mapper.Map<WorkDtoForUpdate>(work);
            return workDto;
        }

        public async Task<ResultDto> UpdateWorkAsync(WorkDtoForUpdate workDto)
        {
            var existingWork = await _manager.Work.GetWorkForUpdateByIdAsync(workDto.WorkId);

            _mapper.Map(workDto, existingWork);

            if(existingWork!.Status == WorkStatus.Passive && workDto.WorkEndDate > DateTime.Now)
            {
                existingWork.Status = WorkStatus.Active;
            }
            else if(existingWork!.Status == WorkStatus.Active && workDto.WorkEndDate < DateTime.Now)
            {
                existingWork.Status = WorkStatus.Passive;
            }

            if (existingWork == null)
                throw new KeyNotFoundException($"{workDto.WorkId} id'sine sahip görev bulunamadı.");

            if (workDto.BroadcastType == "All")
            {
                var internsId = await _manager.Account.GetAllInternsId();

                foreach (var id in internsId)
                {
                    if (existingWork.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id };
                    _manager.Account.Attach(intern);
                    existingWork.Interns?.Add(intern);
                }
            }
            else if (workDto.BroadcastType == "Users")
            {
                if (workDto.UpdatedInternsId != null && workDto.UpdatedInternsId.Any())
                {
                    foreach (var intern in existingWork.Interns!)
                    {
                        if (!workDto.UpdatedInternsId.Contains(intern.Id))
                        {
                            existingWork.Interns?.Remove(intern);
                        }
                    }
                    foreach (var id2 in workDto.UpdatedInternsId)
                    {
                        if (existingWork.Interns!.Where(i => i.Id == id2).Any())
                        {
                            continue;
                        }
                        var intern = new Account { Id = id2 };
                        _manager.Account.Attach(intern);
                        existingWork.Interns?.Add(intern);
                    }
                }
            }
            else if (workDto.BroadcastType == "Department")
            {
                var internsId = await _manager.Account.GelAllInternsOfDepartment(workDto.DepartmentId!.Value);

                foreach (var intern2 in existingWork.Interns!)
                {
                    if (!internsId.Contains(intern2.Id))
                    {
                        existingWork.Interns?.Remove(intern2);
                    }
                }

                foreach (var id in internsId)
                {
                    if (existingWork.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id };
                    _manager.Account.Attach(intern);
                    existingWork.Interns?.Add(intern);
                }
            }
            else if (workDto.BroadcastType == "Section")
            {
                var internsId = await _manager.Account.GelAllInternsOfSection(workDto.SectionId!.Value);

                foreach (var intern2 in existingWork.Interns!)
                {
                    if (!internsId.Contains(intern2.Id))
                    {
                        existingWork.Interns?.Remove(intern2);
                    }
                }

                foreach (var id in internsId)
                {
                    if (existingWork.Interns!.Where(i => i.Id == id).Any())
                    {
                        continue;
                    }
                    var intern = new Account { Id = id! };
                    _manager.Account.Attach(intern);
                    existingWork.Interns?.Add(intern);
                }
            }

            _manager.Work.UpdateWork(existingWork);
            await _manager.SaveAsync();

            return new ResultDto
            {
                Success = true,
                Message = "Görev başarıyla güncellendi.",
                ResultType = "success",
                LoadComponent = "Works"
            };
        }

        public async Task<ResultDto> ChangeStatusAsync(int workId)
        {
            var work = await _manager.Work.GetWorkForUpdateByIdAsync(workId);

            if (work == null)
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");

            if (work.Status == WorkStatus.Passive)
            {
                work.Status = WorkStatus.Active;
            }
            else
            {
                work.Status = WorkStatus.Passive;
            }

            _manager.Work.UpdateWork(work);
            await _manager.SaveAsync();

            return new ResultDto
            {
                Success = true,
                Message = "Görev durumu başarıyla güncellendi.",
                ResultType = "success",
            };
        }
    }
}
