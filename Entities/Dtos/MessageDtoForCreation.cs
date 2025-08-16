using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record MessageDtoForCreation
    {
        public String? MessageTitle { get; set; }
        public String? MessageBody { get; set; }
        public String? BroadcastType { get; init; }
        public int? DepartmentId { get; init; }
        public int? SectionId { get; init; }
        public List<String>? InternsId { get; init; }
    }
}
