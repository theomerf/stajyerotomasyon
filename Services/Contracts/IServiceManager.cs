namespace Services.Contracts
{
    public interface IServiceManager
    {
        IAuthService AuthService { get; }
        ISectionService SectionService { get; }
        IDepartmentService DepartmentService { get; }
        IApplicationService ApplicationService { get; }
        IEventService EventService { get; }
    }
}
