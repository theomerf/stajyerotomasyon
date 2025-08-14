using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext context) : base(context)
        {
        }

        public async Task<List<String>> GetAllInternsId() 
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var internsId = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId), true)
                .Select(i => i.Id)
                .ToListAsync();

            return internsId;
        }
        public async Task<List<string>> GelAllInternsOfDepartment(int departmentId)
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var interns = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId) & u.Section!.DepartmentId == departmentId, false)
                .Select(i => i.Id)
                .ToListAsync();

            return interns;
        }

        public async Task<List<string?>> GelAllInternsOfSection(int sectionId)
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var interns = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId) & u.SectionId == sectionId, false)
                .Select(i => i.Id)
                .ToListAsync();

            return interns!;
        }

        public async Task<int> GetInternsCountAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var count = await _context.UserRoles
                .CountAsync(ur => ur.RoleId == internRoleId);

            return count;
        }

        public async Task<int> GetUsersCountAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var count = await _context.Users
                .Where(u => !_context.UserRoles
                    .Where(ur => ur.RoleId == internRoleId)
                    .Select(ur => ur.UserId)
                    .Contains(u.Id))
                .CountAsync();

            return count;
        }
        public async Task<int> GetLastMonthsInternsCountAsync()
        {
            DateTime lastMonth = DateTime.Now - TimeSpan.FromDays(30);
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var count = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId), false)
                .Where(u => u.InternshipEndDate >= lastMonth && u.InternshipStartDate >= lastMonth)
                .CountAsync();

            return count;
        }

        public async Task<int> GetEndedInternshipCountAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var internsCount = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId) && (u.InternshipEndDate <= DateTime.UtcNow),false)
                .CountAsync();

            return internsCount;
        }

        public async Task<Stats> GetInternshipStatsAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            DateTime now = DateTime.UtcNow;
            DateTime lastMonth = DateTime.UtcNow.AddDays(-30);

            var stats = await FindAllByCondition(
                u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId),
                false)
                .GroupBy(_ => 1)
                .Select(g => new Stats
                {
                    ThisMonthsCount = g.Count(u => u.InternshipStartDate <= now && now <= u.InternshipEndDate ),
                    LastMonthsCount = g.Count(u => u.InternshipEndDate >= lastMonth && u.InternshipStartDate >= lastMonth)
                })
                .FirstOrDefaultAsync();

            return (stats ?? new Stats());
        }

        public async Task<Stats> GetEndedInternshipStatsAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            DateTime now = DateTime.UtcNow;
            DateTime lastMonth = DateTime.UtcNow.AddDays(-30);

            var stats = await FindAllByCondition(
                u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId),
                false)
                .GroupBy(_ => 1)
                .Select(g => new Stats{
                    ThisMonthsCount = g.Count(u => u.InternshipEndDate <= now),
                    LastMonthsCount = g.Count(u => u.InternshipEndDate <= lastMonth)
                })
                .FirstOrDefaultAsync();

             return (stats ?? new Stats());
        }   


        public async Task<IEnumerable<Account>> GetAllInternsAsync(AccountRequestParameters p)
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var interns = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId),false)
                .Include(u => u.Section)
                    .ThenInclude(s => s!.Department)
                .FilteredBySearchTerm(p.SearchTerm ?? "", a => a.FirstName!)
                .FilteredByDepartmentId(p.DepartmentId, a => a.Section!.DepartmentId)
                .SortExtensionForInterns(p.SortBy ?? "")
                .ToPaginate(p.PageNumber, p.PageSize)
                .ToListAsync();


            return interns;
        }

        public async Task<Dictionary<string, int>> GetInternsDepartmentAsync()
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var interns = await (from u in _context.Users
                                 join ur in _context.UserRoles on u.Id equals ur.UserId
                                 where ur.RoleId == internRoleId
                                 join s in _context.Sections on u.SectionId equals s.SectionId
                                 join d in _context.Departments on s.DepartmentId equals d.DepartmentId
                                 group u by d.DepartmentName into g
                                 select new
                                 {
                                     DepartmentName = g.Key!,
                                     Count = g.Count()
                                 })
                                 .ToDictionaryAsync(x => x.DepartmentName, x => x.Count);

            return interns;
        }

        public async Task<int> GetAllInternsCountAsync(AccountRequestParameters p)
        {
            var internRoleId = await _context.Roles
                .Where(r => r.Name == "Stajyer")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var interns = await FindAllByCondition(u => _context.UserRoles.Any(ur => ur.UserId == u.Id && ur.RoleId == internRoleId), false)
                .Include(u => u.Section)
                    .ThenInclude(s => s!.Department)
                .FilteredBySearchTerm(p.SearchTerm ?? "", a => a.FirstName!)
                .FilteredByDepartmentId(p.DepartmentId, a => a.Section!.DepartmentId)
                .CountAsync();

            return interns;
        }

        public async Task<Account?> GetOneInternAsync(string userName)
        {
            var intern = await FindAllByCondition(u => u.UserName == userName, true)
                .Include(u => u.Section)
                    .ThenInclude(s => s!.Department)
                .FirstOrDefaultAsync();

            return intern;
        }

        public async Task<String?> GetOneUsersNameAsync(string userId)
        {
            var intern = await FindByCondition(u => u.Id == userId, true)
                .Select(u => u!.FirstName + ' ' + u.LastName)
                .FirstOrDefaultAsync();

            return intern;
        }

        public async Task<IEnumerable<AccountDtoForSearch>> SearchInterns(string userName)
        {
            var interns = await FindAll(false)
                  .FilteredBySearchTerm(userName ?? "", a => a.FirstName!)
                  .Select(i => new AccountDtoForSearch()
                  {
                      Id = i.Id,
                      FirstName = i.FirstName,
                      LastName = i.LastName,
                      Email = i.Email,
                      UserName = i.UserName,
                  })
                  .ToListAsync();

            return interns;
        }

        public async Task<IEnumerable<AccountDtoForSearch>> GetInternsByIds(List<string> internsIds)
        {
            var interns = await FindAll(false)
                .Where(u => internsIds.Contains(u.Id))
                .Select(i => new AccountDtoForSearch()
                {
                     Id = i.Id,
                     FirstName = i.FirstName,
                     LastName = i.LastName,
                     Email = i.Email,
                     UserName = i.UserName,
                })
                .ToListAsync();

            return interns;
        }

    }
}
