namespace Repositories.Contracts
{
    public interface IRepositoryManager
    {
        ISectionRepository Section { get; }
        IDepartmentRepository Department { get; }
        IAccountRepository Account { get; }
        IApplicationRepository Application { get; }
        INoteRepository Note { get; }
        IEventRepository Event { get; }
        void Save();
        Task SaveAsync();
    }
}
