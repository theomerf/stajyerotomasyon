using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public record ResetPasswordDto
    {
        public string? UserName { get; init; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre gerekli")]
        public string? Password { get; init; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Lütfen şifreyi tekrar giriniz.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmedi.")]
        public string? ConfirmPassword { get; init; }
    }
}
