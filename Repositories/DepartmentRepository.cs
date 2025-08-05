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
    public sealed class DepartmentRepository : RepositoryBase<Department>, IDepartmentRepository
    {

        public DepartmentRepository(RepositoryContext context) : base(context)
        {
        }

        public void CreateDepartment(Department department)
        {
            Create(department);
        }

        public void DeleteDepartment(Department department)
        {
            Remove(department);
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await FindAll(false)
                .OrderBy(d => d.DepartmentId)
                .Include(d => d.Sections)
                .ToListAsync();
        }

        public async Task<int> GetAllDepartmentsCountAsync()
        {
            return await FindAll(false)
                .CountAsync();
        }

        public void UpdateDepartment(Department department)
        {
            Update(department);
        }

        public async Task<Department?> GetDepartmentByIdAsync(int departmentId)
        {
            return await FindByCondition(d => d.DepartmentId.Equals(departmentId), false)
                .FirstOrDefaultAsync();
        }
    }
}
