using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestParameters
{
    public class AccountRequestParameters :RequestParameters
    {
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public AccountRequestParameters() : this(1, 6)
        {
        }
        public AccountRequestParameters(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
