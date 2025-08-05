using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionDto>> GetAllSectionsAsync();
        Task<string> GetAllSectionsCountAsync();
        Task<ResultDto> CreateSectionsAsync(string departmentId, List<string> sectionNames);
        Task<ResultDto> DeleteSectionAsync(int departmentId);
        Task<ResultDto> UpdateSectionAsync(SectionDto sectionDto);
    }
}
