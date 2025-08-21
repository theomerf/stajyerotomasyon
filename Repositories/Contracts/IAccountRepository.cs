using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Contracts
{
    public interface IAccountRepository : IRepositoryBase<Account>
    {
        Task<int> GetInternsCountAsync();
        Task<List<String>> GetAllInternsId();
        Task<IEnumerable<AccountDtoForSearch>> SearchInterns(string userName);
        Task<List<string?>> GelAllInternsOfSection(int sectionId);
        Task<String?> GetOneUserForNavbarAsync(string userName);
        Task<List<string>> GelAllInternsOfDepartment(int departmentId);
        Task<Stats> GetEndedInternshipStatsAsync();
        Task<int> GetUsersCountAsync();
        Task<Stats> GetInternshipStatsAsync();
        Task<IEnumerable<Account>> GetAllInternsAsync(AccountRequestParameters p);
        Task<int> GetAllInternsCountAsync(AccountRequestParameters p);
        Task<int> GetEndedInternshipCountAsync();
        Task<Dictionary<string, int>> GetInternsDepartmentAsync();
        Task<Account?> GetOneInternAsync(string userName);
        Task<int> GetLastMonthsInternsCountAsync();
        Task<String?> GetOneUsersNameAsync(string userId);
        Task<IEnumerable<AccountDtoForSearch>> GetInternsByIds(List<string> internsIds);
    }
}
