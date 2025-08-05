using Entities.Dtos;
using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
        Task<string> GetAllDepartmentsCountAsync();
        Task<ResultDto> CreateDepartmentAsync(DepartmentDto departmentDto);
        Task<ResultDto> DeleteDepartmentAsync(int departmentId);
        Task<ResultDto> UpdateDepartmentAsync(DepartmentDto departmentDto);
    }
}
