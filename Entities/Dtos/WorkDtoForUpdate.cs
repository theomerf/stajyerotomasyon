using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record WorkDtoForUpdate : WorkDtoForCreation
    {
        public int WorkId { get; init; }
        public List<String>? PhotosToDelete { get; set; }
        public List<String>? UpdatedInternsId { get; set; }
    }
}
