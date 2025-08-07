using Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record WorkDtoForCreation : WorkDto
    {
        [Required(ErrorMessage = "Görev verilecek kişiler gerekli.")]
        public List<String>? InternsId { get; init; }
        public ICollection<Account>? Interns { get; init; }
        public String? TaskMasterId { get; init; }
        public Account? TaskMaster { get; init; }
    }
}
