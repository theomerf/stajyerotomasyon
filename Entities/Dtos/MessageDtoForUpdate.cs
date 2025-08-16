using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record MessageDtoForUpdate : MessageDtoForCreation
    {
        public int MessageId { get; init; }
        public List<String>? UpdatedInternsId { get; init; }
    }
}
