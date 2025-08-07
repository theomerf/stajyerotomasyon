using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestParameters
{
    public class WorkRequestParameters : RequestParameters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public WorkRequestParameters() : this(1, 6)
        {
        }
        public WorkRequestParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
