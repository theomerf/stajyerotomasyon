using Repositories.Contracts;

namespace Repositories
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _context;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IReportRepository _reportRepository;
        private readonly IWorkRepository _workRepository;
        private readonly IMessageRepository _messageRepository;


        public RepositoryManager(RepositoryContext context, IDepartmentRepository departmentRepository, ISectionRepository sectionRepository, IAccountRepository accountRepository, IApplicationRepository applicationRepository, INoteRepository noteRepository, IEventRepository eventRepository, IReportRepository reportRepository, IWorkRepository workRepository, IMessageRepository messageRepository)
        {
            _context = context;
            _departmentRepository = departmentRepository;
            _sectionRepository = sectionRepository;
            _accountRepository = accountRepository;
            _applicationRepository = applicationRepository;
            _noteRepository = noteRepository;
            _eventRepository = eventRepository;
            _reportRepository = reportRepository;
            _workRepository = workRepository;
            _messageRepository = messageRepository;
        }

        public IDepartmentRepository Department => _departmentRepository;
        public ISectionRepository Section => _sectionRepository;
        public IAccountRepository Account => _accountRepository;
        public IApplicationRepository Application => _applicationRepository;
        public INoteRepository Note => _noteRepository;
        public IEventRepository Event => _eventRepository;
        public IReportRepository Report => _reportRepository;
        public IWorkRepository Work => _workRepository;
        public IMessageRepository Message => _messageRepository;

        public void Save()
        {
            _context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
