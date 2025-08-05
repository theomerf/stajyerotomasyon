using Entities.Dtos;
using Entities.Models;

namespace Services.Contracts
{
    public interface IWorkService
    {
        Task<IEnumerable<WorkDto?>> GetAllWorksAsync();
        Task<WorkDto?> GetWorkByIdAsync(int workId);
        Task<IEnumerable<WorkDto?>> GetAllWorksOfOneUserAsync(string userId);
        Task<int> GetAllWorksCountAsync();
        Task<int> GetAllWorksCountOfOneUser(string userId);
        Task<ResultDto> CreateWorkAsync(WorkDto workDto);
        Task<ResultDto> DeleteWorkAsync(int workId);
        Task<ResultDto> UpdateWorkAsync(WorkDto workDto);
    }
}
