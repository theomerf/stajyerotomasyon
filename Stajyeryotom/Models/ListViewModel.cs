using Entities.Dtos;

namespace Stajyeryotom.Models
{
    public class ListViewModel<T>
    {
        public IList<T>? List { get; set; }
        public Pagination? Pagination { get; set; }
        public int TotalCount => List?.Count ?? 0;
    }

}
