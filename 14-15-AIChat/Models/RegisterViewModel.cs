using System.ComponentModel.DataAnnotations;

namespace _14_15_AIChat.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Ad soyad zorunludur.")]
        [StringLength(60, ErrorMessage = "Ad soyad en fazla 60 karakter olabilir.")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = "";
    }
}