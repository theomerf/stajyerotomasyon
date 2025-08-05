using AutoMapper;
using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Contracts;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthManager : IAuthService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<Account> _userManager;
        private readonly IRepositoryManager _manager;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public AuthManager(RoleManager<IdentityRole> roleManager, UserManager<Account> userManager, IMapper mapper, IRepositoryManager manager, IMemoryCache cache)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _mapper = mapper;
            _manager = manager;
            _cache = cache;
        }

        public IEnumerable<IdentityRole> Roles => _roleManager.Roles;

        public async Task<string> GetAllUsersCountAsync()
        {
            string cacheKey = "usersCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData!.ToString() ?? "0";
            }

            var count = await _manager.Account.GetUsersCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, count, cacheOptions);

            return count.ToString();
        } 

        public async Task<IdentityResult> CreateUserAsync(AccountDtoForCreation userDto)
        {
            var user = _mapper.Map<Account>(userDto);
            var newUserNo = await GenerateNewUserNumberAsync();
            user.UserName = newUserNo;

            var result = await _userManager.CreateAsync(user, userDto.Password!);

            if (!result.Succeeded)
            {
                Console.WriteLine("Kullanıcı oluşturulamadı.");
                return result;
            }
            if (userDto.Roles?.Count > 0)
            {
                var roleResult = await _userManager.AddToRolesAsync(user, userDto.Roles!);
                if (!roleResult.Succeeded)
                {
                    Console.WriteLine("Rol eklenemedi.");
                    return roleResult;
                }
            }

            if (user.InternshipStartDate != DateTime.MinValue) 
            {
                _cache.Remove("internshipStats");
                _cache.Remove("internsCount");
                _cache.Remove("internsDepartment");
            }
            else if(user.InternshipStartDate == DateTime.MinValue)
            {
                _cache.Remove("usersCount");
            }
            else if (user.InternshipEndDate <= DateTime.Now && user.InternshipStartDate <= DateTime.Now)
            {
                _cache.Remove("endedInternshipStats");
            }

            return result;
        }

        public async Task<string> GetInternsCountAsync()
        {
            string cacheKey = "internsCount";

            if (_cache.TryGetValue(cacheKey, out int? cachedData))
            {
                return cachedData.ToString() ?? "0";
            }
            var internsCount = await _manager.Account.GetInternsCountAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, internsCount, cacheOptions);
            return internsCount.ToString();
        }

        public async Task<StatsDto> GetEndedInternshipStatsAsync()
        {
            string cacheKey = "endedInternshipStats";

            if (_cache.TryGetValue(cacheKey, out StatsDto? cachedData))
            {
                return cachedData ?? new StatsDto();
            }

            var stats = await _manager.Account.GetEndedInternshipStatsAsync();
            var statsDto = _mapper.Map<StatsDto>(stats);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, statsDto, cacheOptions);

            return statsDto;
        }

        public async Task<StatsDto> GetInternshipStatsAsync()
        {
            string cacheKey = "internshipStats";

            if (_cache.TryGetValue(cacheKey, out StatsDto? cachedData))
            {
                return cachedData ?? new StatsDto();
            }

            var stats = await _manager.Account.GetInternshipStatsAsync();
            var statsDto = _mapper.Map<StatsDto>(stats);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, statsDto, cacheOptions);

            return statsDto;
        }

        public async Task<string> GetLastMonthsInternsCountAsync()
        {
            var intersCount = await _manager.Account.GetLastMonthsInternsCountAsync();
            return intersCount.ToString();
        }

        public async Task<int> GetAllInternsCountAsync(AccountRequestParameters p)
        {
            var intersCount = await _manager.Account.GetAllInternsCountAsync(p);
            return intersCount;
        }


        public async Task<IdentityResult> DeleteOneUserAsync(string? userName)
        {
            var user = await GetOneUserForServiceAsync(userName!);
            if (user.InternshipStartDate != DateTime.MinValue)
            {
                _cache.Remove("internshipStats");
                _cache.Remove("internsCount");
                _cache.Remove("internsDepartment");
            }
            else if(user.InternshipEndDate == DateTime.MinValue)
            {
                _cache.Remove("usersCount");
            }
            else if (user.InternshipEndDate <= DateTime.Now && user.InternshipStartDate <= DateTime.Now)
            {
                _cache.Remove("endedInternshipStats");
            } 
            
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<AccountDto>> GetAllInternsAsync(AccountRequestParameters p)
        {
            Console.WriteLine("------------------------BACKENDE STAJYER SORGUSU GÖNDERİLDİ-------------------");
            var interns = await _manager.Account.GetAllInternsAsync(p);
            var internsDto = _mapper.Map<IEnumerable<AccountDto>>(interns);
            return internsDto;
        }

        public async Task<IEnumerable<AccountDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<AccountDto>>(users);
        }

        public async Task<Account> GetOneUserForServiceAsync(string userName)
        {
            var user = await _manager.Account.GetOneInternAsync(userName!);
            if (user != null)
            {
                return user;
            }
            throw new Exception("Kullanıcı bulunamadı.");
        }

        public async Task<AccountDto> GetOneUserAsync(string? userName)
        {
            var user = await _userManager.FindByNameAsync(userName ?? "");

            if (user != null)
            {
                var userDto = _mapper.Map<AccountDto>(user);
                return userDto;
            }
            throw new Exception("Kullanıcı bulunamadı.");
        }

        public async Task<AccountDtoForUpdate> GetOneUserForUpdateAsync(string? userName)
        {
            var user = await GetOneUserForServiceAsync(userName!);

            var userDtoForUpdate = _mapper.Map<AccountDtoForUpdate>(user);
            userDtoForUpdate.RolesList = new HashSet<string?>(Roles.Select(r => r.Name).ToList());
            userDtoForUpdate.Roles = new HashSet<string?>(await _userManager.GetRolesAsync(user));
            return userDtoForUpdate;
        }

        public async Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model)
        {
            var user = await GetOneUserForServiceAsync(model.UserName!);

            await _userManager.RemovePasswordAsync(user);

            if (model.Password == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Şifre boş olamaz." });
            }

            var result = await _userManager.AddPasswordAsync(user, model.Password);
            return result;
        }

 /*       public async Task<IdentityResult> ChangePasswordAsync(ChangePasswordDto model)
        {
            var user = await GetOneUserForServiceAsync(model.UserName);

            if (model.Password != null && model.NewPassword != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);
                return result;
            }
            return IdentityResult.Failed(new IdentityError { Description = "Şifre değişikliği başarısız oldu." });
        }*/

        public async Task<IdentityResult> UpdateAsync(AccountDtoForUpdate userDtoForUpdate)
        {
            var user = await GetOneUserForServiceAsync(userDtoForUpdate.UserName!);

            user.FirstName = userDtoForUpdate.FirstName;
            user.LastName = userDtoForUpdate.LastName;
            user.PhoneNumber = userDtoForUpdate.PhoneNumber;
            user.Email = userDtoForUpdate.Email;
            user.InternshipStartDate = userDtoForUpdate.InternshipStartDate;
            user.InternshipEndDate = userDtoForUpdate.InternshipEndDate;
            user.SectionId = userDtoForUpdate.SectionId;
            user.Section!.DepartmentId = userDtoForUpdate.DepartmentId;

            var result = await _userManager.UpdateAsync(user);
            if (userDtoForUpdate.Roles?.Count > 0)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRolesAsync(user, userDtoForUpdate.Roles!);
            }
            else if (user.InternshipEndDate <= DateTime.Now && user.InternshipStartDate <= DateTime.Now)
            {
                _cache.Remove("endedInternshipStats");
            }
            else if (user.SectionId != userDtoForUpdate.SectionId) 
            {
                _cache.Remove("internsDepartment");
            }
            return result;
        }

        public async Task<IdentityResult> UpdateAvatarAsync(AccountDtoForUpdate userDtoForUpdate)
        {
            var user = await GetOneUserForServiceAsync(userDtoForUpdate.UserName!);

            if (userDtoForUpdate.ProfilePictureUrl != null)
            {
                user.ProfilePictureUrl = userDtoForUpdate.ProfilePictureUrl;
            }

            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<string> GenerateNewUserNumberAsync()
        {
            string? lastUserNo = await _userManager.Users
                .OrderByDescending(u => u.UserName)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            int lastNumber = 0;
            if (!string.IsNullOrWhiteSpace(lastUserNo))
            {
                lastNumber = int.Parse(lastUserNo);
            }

            int newNumber = lastNumber + 1;
            return newNumber.ToString("D5");
        }

        public async Task<string> GetEndedInternshipCountAsync()
        {
            var count = await _manager.Account.GetEndedInternshipCountAsync();
            return count.ToString();
        }

        public async Task<Dictionary<string, int>> GetInternsDepartmentAsync()
        {
            string cacheKey = "internsDepartment";

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, int>? cachedData))
            {
                return cachedData ?? new Dictionary<string, int>();
            }
            var countOfInternsByDepartment = await _manager.Account.GetInternsDepartmentAsync();

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));

            _cache.Set(cacheKey, countOfInternsByDepartment, cacheOptions);

            return countOfInternsByDepartment;
        }

        public async Task<String?> GetOneUsersNameAsync(string userId)
        {
            var name = await _manager.Account.GetOneUsersNameAsync(userId);
            return name;
        }
    }
}
