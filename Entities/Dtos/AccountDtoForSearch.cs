using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record AccountDtoForSearch
    {
        public String? Id { get; init; }
        public String? FirstName { get; init; }
        public String? LastName { get; init; }
        public String? Email { get; init; }
        public String? UserName { get; init; }
    }
}
