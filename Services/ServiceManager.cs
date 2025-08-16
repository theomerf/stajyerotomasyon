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
        private readonly IReportService _reportService;
        private readonly IWorkService _workService;
        private readonly IMessageService _messageService;

        public ServiceManager(IAuthService authService, ISectionService sectionService, IDepartmentService departmentService, IApplicationService applicationService, IEventService eventService, IReportService reportService, IWorkService workService, IMessageService messageService)
        {
            _authService = authService;
            _sectionService = sectionService;
            _departmentService = departmentService;
            _applicationService = applicationService;
            _eventService = eventService;
            _reportService = reportService;
            _workService = workService;
            _messageService = messageService;
        }

        public IAuthService AuthService => _authService;
        public ISectionService SectionService => _sectionService;
        public IDepartmentService DepartmentService => _departmentService;
        public IApplicationService ApplicationService => _applicationService;
        public IEventService EventService => _eventService;
        public IReportService ReportService => _reportService;
        public IWorkService WorkService => _workService;
        public IMessageService MessageService => _messageService;   
    }
}
