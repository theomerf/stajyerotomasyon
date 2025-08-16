using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record WorkDto
    {
        public int WorkId { get; init; }
        public String? WorkName { get; init; }
        public String? WorkDescription { get; init; }
        public List<String>? ImageUrls { get; set; }
        public DateTime WorkStartDate { get; init; }
        public DateTime WorkEndDate { get; init; }
        public String? TaskMasterName { get; init; }
        public int InternsCount { get; init; }
        public int ReportsCount { get; init; }
        public String? Status { get; init; }
    }
}
