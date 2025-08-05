using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record AccountDtoForCreation : AccountDto
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre gerekli.")]
        public string? Password { get; init; }
    }
}
