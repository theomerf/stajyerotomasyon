using System.ComponentModel.DataAnnotations;

namespace Stajyeryotom.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Kullanıcı no gereklidir.")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Şifre gereklidir.")]
        public string? Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
