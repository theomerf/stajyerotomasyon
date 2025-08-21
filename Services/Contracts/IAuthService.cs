using Entities.Dtos;
using Entities.RequestParameters;
using Microsoft.AspNetCore.Identity;

namespace Services.Contracts
{
    public interface IAuthService
    {
        IEnumerable<IdentityRole> Roles { get; }
        Task<IEnumerable<AccountDto>> GetAllUsersAsync();
        Task<List<String>> GetAllInternsId();
        Task<List<string?>> GelAllInternsOfSection(int sectionId);
        Task<List<string?>> GelAllInternsOfDepartment(int departmentId);
        Task<StatsDto> GetEndedInternshipStatsAsync();
        Task<String?> GetOneUserForNavbarAsync(string userName);
        Task<StatsDto> GetInternshipStatsAsync();
        Task<string> GetAllUsersCountAsync();
        Task<IdentityResult> CreateUserAsync(AccountDtoForCreation userDto);
        Task<AccountDto> GetOneUserAsync(string? userName);
        Task<AccountDtoForUpdate> GetOneUserForUpdateAsync(string? userName);
        Task<IdentityResult> UpdateAsync(AccountDtoForUpdate userDto);
        Task<IdentityResult> UpdateAvatarAsync(AccountDtoForUpdate userDto);
        Task<IdentityResult> ResetPasswordAsync(ResetPasswordDto model);
        Task<IdentityResult> DeleteOneUserAsync(string? userName);
        Task<string> GenerateNewUserNumberAsync();
        Task<int> GetAllInternsCountAsync(AccountRequestParameters p);
        Task<string> GetInternsCountAsync();
        Task<IEnumerable<AccountDtoForSearch>> SearchInterns(string userName);
        Task<String?> GetOneUsersNameAsync(string userId);
        Task<Dictionary<string, int>> GetInternsDepartmentAsync();
        Task<IEnumerable<AccountDto>> GetAllInternsAsync(AccountRequestParameters p);
        Task<string> GetEndedInternshipCountAsync();
        Task<string> GetLastMonthsInternsCountAsync();
        Task<IEnumerable<AccountDtoForSearch>> GetInternsByIds(List<string> internsIds);
        Task<IdentityResult> ChangeStatus(string userName);

    }
}
