using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Work
    {
        public int WorkId { get; set; }
        public String? WorkName { get; set; }
        public String? WorkDescription { get; set; }
        public List<String>? ImageUrls { get; set; }
        public DateTime? WorkStartDate { get; set; }
        public DateTime? WorkEndDate { get; set; }
        public String? BroadcastType { get; init; }
        public Account? TaskMaster { get; set; }
        public String? TaskMasterId { get; set; }
        public int? DepartmentId { get; init; }
        public int? SectionId { get; init; }
        public ICollection<Account>? Interns { get; set; }
        public ICollection<Report>? Reports { get; set; }
    }
}
