using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Message
    {
        public int MessageId { get; set; }
        public String? MessageTitle { get; set; }
        public String? MessageBody { get; set; }
        public DateTime? CreatedAt = DateTime.Now;
        public String? BroadcastType { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public ICollection<Account>? Interns { get; set; }

    }
}
