using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IDepartmentRepository : IRepositoryBase<Department>
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<int> GetAllDepartmentsCountAsync();
        Task<Department?> GetDepartmentByIdAsync(int departmentId);
        void CreateDepartment(Department department);
        void DeleteDepartment(Department department);
        void UpdateDepartment(Department department);  
    }
}
