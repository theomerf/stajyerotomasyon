using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public String? ReportTitle { get; set; }
        public String? ReportContent { get; set; }
        public List<String>? ImageUrls { get; set; }
        public String? Status { get; set; } = "NotRead";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Account? Account { get; set; }
        public String? AccountId { get; set; }
        public Work? Work { get; set; }
        public int? WorkId { get; set; } 
    }
}
