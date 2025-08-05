using Services.Contracts;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IAuthService _authService;
        private readonly ISectionService _sectionService;
        private readonly IDepartmentService _departmentService;
        private readonly IApplicationService _applicationService;
        private readonly IEventService _eventService;

        public ServiceManager(IAuthService authService, ISectionService sectionService, IDepartmentService departmentService, IApplicationService applicationService, IEventService eventService)
        {
            _authService = authService;
            _sectionService = sectionService;
            _departmentService = departmentService;
            _applicationService = applicationService;
            _eventService = eventService;
        }

        public IAuthService AuthService => _authService;
        public ISectionService SectionService => _sectionService;
        public IDepartmentService DepartmentService => _departmentService;
        public IApplicationService ApplicationService => _applicationService;
        public IEventService EventService => _eventService;
    }
}
