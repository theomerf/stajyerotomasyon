using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
    public class WorkManager : IWorkService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;

        public WorkManager(IRepositoryManager manager, IMapper mapper)
        {
            _manager = manager;
            _mapper = mapper;
        }

        public async Task<ResultDto> CreateWorkAsync(WorkDto workDto)
        {
            var work = _mapper.Map<Work>(workDto);
            _manager.Work.Create(work);
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
                Message = "Görev başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Works"
            };
            return result;
        }

        public async Task<IEnumerable<WorkDto?>> GetAllWorksAsync()
        {
            var works = await _manager.Work.GetAllWorksAsync();
            var worksDto = _mapper.Map<IEnumerable<WorkDto>>(works);

            return worksDto;
        }

        public async Task<int> GetAllWorksCountAsync()
        {
            var count = await _manager.Work.GetAllWorksCountAsync();
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

        public async Task<ResultDto> UpdateWorkAsync(WorkDto workDto)
        {
            var work = _mapper.Map<Work>(workDto);
            _manager.Work.UpdateWork(work);
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
    }
}
