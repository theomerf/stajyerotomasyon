using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface ISectionRepository : IRepositoryBase<Section>
    {
        Task<IEnumerable<Section>> GetAllSectionsAsync();
        Task<int> GetAllSectionsCountAsync();
        Task<Section?> GetSectionByIdAsync(int sectionId);
        void CreateSections(IEnumerable<Section> sections);
        void DeleteSection(Section section);
        void UpdateSection(Section section);
    }
}
