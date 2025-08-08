using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Section
    {
        public int SectionId { get; set; }
        public String? SectionName { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public ICollection<Account>? Users { get; set; }
        public ICollection<Application>? Applications { get; set; }
    }
}
