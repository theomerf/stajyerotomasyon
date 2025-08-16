using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class MessageDto
    {
        public int MessageId { get; init; }
        public String? MessageTitle { get; init; }
        public String? MessageBody { get; init; }
        public DateTime? CreatedAt { get; init; }
        public int InternsCount { get; init; }

    }
}
