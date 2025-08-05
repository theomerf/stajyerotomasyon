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


        public RepositoryManager(RepositoryContext context, IDepartmentRepository departmentRepository, ISectionRepository sectionRepository, IAccountRepository accountRepository, IApplicationRepository applicationRepository, INoteRepository noteRepository, IEventRepository eventRepository)
        {
            _context = context;
            _departmentRepository = departmentRepository;
            _sectionRepository = sectionRepository;
            _accountRepository = accountRepository;
            _applicationRepository = applicationRepository;
            _noteRepository = noteRepository;
            _eventRepository = eventRepository;
        }

        public IDepartmentRepository Department => _departmentRepository;
        public ISectionRepository Section => _sectionRepository;
        public IAccountRepository Account => _accountRepository;
        public IApplicationRepository Application => _applicationRepository;
        public INoteRepository Note => _noteRepository;
        public IEventRepository Event => _eventRepository;

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
