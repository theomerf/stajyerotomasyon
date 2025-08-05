using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class SectionRepository : RepositoryBase<Section>, ISectionRepository
    {
        public SectionRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateSections(IEnumerable<Section> sections)
        {
            foreach (var section in sections)
                Create(section);
        }

        public void DeleteSection(Section section)
        {
            Remove(section);
        }

        public async Task<IEnumerable<Section>> GetAllSectionsAsync()
        {
            return await FindAll(false)
                .OrderBy(s => s.SectionId)
                .ToListAsync();
        }

        public async Task<int> GetAllSectionsCountAsync()
        {
            return await FindAll(false)
                .CountAsync();
        }

        public async Task<Section?> GetSectionByIdAsync(int sectionId)
        {
            return await FindByCondition(s => s.SectionId.Equals(sectionId), false)
                .FirstOrDefaultAsync();
        }

        public void UpdateSection(Section section)
        {
            Update(section);
        }
    }
}
