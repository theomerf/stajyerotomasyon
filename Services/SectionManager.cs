using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Services
{
    public class SectionManager : ISectionService
    {
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public SectionManager(IRepositoryManager manager, IMapper mapper, IMemoryCache cache)
        {
            _manager = manager;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ResultDto> CreateSectionsAsync(string departmentId, List<string> sectionNames)
        {
            var sections = sectionNames.Select(name => new Entities.Models.Section
            {
                DepartmentId = int.Parse(departmentId),
                SectionName = name
            }).ToList();

            _manager.Section.CreateSections(sections);

            _cache.Remove("sections");
            _cache.Remove("departments");
            _cache.Remove("sectionsCount");
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


        private async Task <Entities.Models.Section> GetOneSectionForServiceAsync(int sectionId)
        {
            var section = await _manager.Section.GetSectionByIdAsync(sectionId);
            if (section != null)
            {
                return section;
            }
            throw new KeyNotFoundException($"{sectionId} id'sine sahip bölüm bulunamadı.");
        }

        public async Task<ResultDto> DeleteSectionAsync(int sectionId)
        {
            var section = await GetOneSectionForServiceAsync(sectionId);
            _manager.Section.DeleteSection(section);

            _cache.Remove("sections");
            _cache.Remove("sectionsCount");
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Bölüm başarıyla silindi.",
                ResultType = "success",
                LoadComponent = "Company"
            };
            return result;
        }

        public async Task<IEnumerable<SectionDto>> GetAllSectionsAsync()
        {
            string cacheKey = "sections";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<SectionDto>? cachedData))
            {
                return cachedData!;
            }

            var sections = await _manager.Section.GetAllSectionsAsync();
            var sectionsDto = _mapper.Map<IEnumerable<SectionDto>>(sections);

            var cacheOptions = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, sectionsDto, cacheOptions);


            return sectionsDto;
        }

        public async Task<string> GetAllSectionsCountAsync()
        {
            string cacheKey = "sectionsCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData.ToString() ?? "0";
            }

            var sectionsCount = await _manager.Section.GetAllSectionsCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
              .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, sectionsCount, cacheOptions);


            return sectionsCount.ToString();
        }

        public async Task<ResultDto> UpdateSectionAsync(SectionDto sectionDto)
        {
            var section = _mapper.Map<Entities.Models.Section>(sectionDto);
            _manager.Section.Update(section);

            _cache.Remove("sections");
            await _manager.SaveAsync();

            var result = new ResultDto()
            {
                Success = true,
                Message = "Bölüm başarıyla güncellendi..",
                ResultType = "success",
                LoadComponent = "Company"
            };
            return result;

        }
    }
}
