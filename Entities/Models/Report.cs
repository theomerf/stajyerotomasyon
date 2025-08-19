using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Report
    {
        public int ReportId { get; set; }
        public String? ReportTitle { get; set; }
        public String? ReportContent { get; set; }
        public List<String>? ImageUrls { get; set; }
        public ReportStatus Status { get; set; } = ReportStatus.NotRead;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ReportType ReportType { get; set; }
        public Account? Account { get; set; }
        public String? AccountId { get; set; }
        public Work? Work { get; set; }
        public int? WorkId { get; set; } 
    }

    public enum ReportType
    {
        Daily = 0,
        Work = 1,
    }

    public enum ReportStatus
    {
        NotRead = 0,
        Read = 1,
    }
}
