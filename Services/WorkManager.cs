using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class WorkManager : IWorkService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly RepositoryContext _context;

        public WorkManager(IRepositoryManager manager, IMapper mapper, RepositoryContext context)
        {
            _manager = manager;
            _mapper = mapper;
            _context = context;
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

            var result = new ResultDto()
            {
                Success = true,
                Message = "Görev başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Works"
            };
            return result;
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

        public async Task<WorkDtoForUpdate?> GetWorkForUpdateByIdAsync(int workId)
        {
            var work = await _manager.Work.GetWorkForUpdateByIdAsync(workId);
            if(work == null)
            {
                throw new KeyNotFoundException($"{workId} id'sine sahip görev bulunamadı.");
            }

            var workDto = _mapper.Map<WorkDtoForUpdate>(work);
            return workDto;
        }

        public async Task<ResultDto> UpdateWorkAsync(WorkDtoForUpdate workDto)
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

            _manager.Work.UpdateWork(work);
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Rapor başarıyla güncellendi.",
                ResultType = "success",
                LoadComponent = "Reports"
            };

            return result;
        }
    }
}
