using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestParameters
{
    public class ReportRequestParameters : RequestParameters
    {
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public String? StartDate { get; set; }
        public String? EndDate { get; set; }
        public String? Status { get; set; }
        public String? Type { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ReportRequestParameters() : this(1, 6)
        {
        }
        public ReportRequestParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
