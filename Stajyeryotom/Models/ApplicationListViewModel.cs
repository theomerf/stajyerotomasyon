using Entities.Dtos;

namespace Stajyeryotom.Models
{
    public class ApplicationListViewModel
    {
        public IEnumerable<ApplicationDto>? Applications { get; set; }
        public Pagination? Pagination { get; set; }
        public int? TotalCount => Applications?.Count();
    }
}
