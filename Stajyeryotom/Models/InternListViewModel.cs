using Entities.Dtos;

namespace Stajyeryotom.Models
{
    public class InternListViewModel
    {
        public IEnumerable<AccountDto>? Interns { get; set; }
        public Pagination? Pagination { get; set; }
        public int? TotalCount => Interns?.Count();
    }
}
