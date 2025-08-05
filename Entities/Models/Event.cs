using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public String? Title { get; set; }
        public TimeSpan Time { get; set; }
        public String? Type { get; set; }
        public String? Description { get; set; }
        public DateTime Date { get; set; }
    }
}
