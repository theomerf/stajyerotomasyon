using Entities.Dtos;
using Entities.Models;
using Entities.RequestParameters;

namespace Services.Contracts
{
    public interface IWorkService
    {
        Task<IEnumerable<WorkDto?>> GetAllWorksAsync(WorkRequestParameters p);
        Task<WorkDto?> GetWorkByIdAsync(int workId);
        Task<IEnumerable<WorkDto?>> GetAllWorksOfOneUserAsync(string userId);
        Task<int> GetAllWorksCountAsync();
        Task<int> GetWorksCountAsync(WorkRequestParameters p);
        Task<int> GetAllWorksCountOfOneUser(string userId);
        Task<ResultDto> CreateWorkAsync(WorkDtoForCreation workDto);
        Task<ResultDto> DeleteWorkAsync(int workId);
        Task<ResultDto> UpdateWorkAsync(WorkDto workDto);
    }
}
