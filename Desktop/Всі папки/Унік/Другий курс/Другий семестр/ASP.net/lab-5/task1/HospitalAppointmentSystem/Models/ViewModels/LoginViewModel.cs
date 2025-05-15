using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть Email")]
        [EmailAddress(ErrorMessage = "Невірний формат Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; } = "/";
    }
}