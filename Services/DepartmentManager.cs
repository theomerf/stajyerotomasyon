using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;
using System.Threading.Tasks;

namespace Services
{
    public class DepartmentManager : IDepartmentService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public DepartmentManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> CreateDepartmentAsync(DepartmentDto departmentDto)
        {
            var department = _mapper.Map<Department>(departmentDto);
            _manager.Department.CreateDepartment(department);

            _cache.Remove("departments");
            _cache.Remove("departmentsCount");
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Departman başarıyla oluşturuldu.",
                ResultType = "success",
                LoadComponent = "Company"
            };
            return result;
        }

        private async Task<Department> GetOneDepartmentForServiceAsync(int departmentId)
        {
            var department = await _manager.Department.GetDepartmentByIdAsync(departmentId);
            if(department != null)
            {
                return department;
            }
            throw new KeyNotFoundException($"{departmentId} id'sine sahip departman bulunamadı.");
        }

        public async Task<ResultDto> DeleteDepartmentAsync(int departmentId)
        {
            var department = await GetOneDepartmentForServiceAsync(departmentId);
            _manager.Department.DeleteDepartment(department);

            _cache.Remove("departments");
            _cache.Remove("departmentsCount");
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Departman başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Company"
            };
            return result;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
        {
            string cacheKey = "departments";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<DepartmentDto>? cachedData))
            {
                return cachedData!;
            }

            var departments = await _manager.Department.GetAllDepartmentsAsync();
            var departmentsDto = _mapper.Map<IEnumerable<DepartmentDto>>(departments);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, departmentsDto, cacheOptions);

            return departmentsDto;
        }

        public async Task<string> GetAllDepartmentsCountAsync()
        {
            string cacheKey = "departmentsCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData!.ToString() ?? "0";
            }

            var count = await _manager.Department.GetAllDepartmentsCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count.ToString();
        }

        public async Task<ResultDto> UpdateDepartmentAsync(DepartmentDto departmentDto)
        {
            var department = _mapper.Map<Department>(departmentDto);
            _manager.Department.Update(department);

            _cache.Remove("departments");
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Departman başarıyla güncellendi.",
                ResultType = "success",
                LoadComponent = "Company"
            };
            return result;
        }
    }
}
