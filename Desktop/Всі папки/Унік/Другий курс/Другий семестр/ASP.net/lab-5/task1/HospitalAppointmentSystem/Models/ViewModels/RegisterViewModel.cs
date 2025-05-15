using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введіть прізвище")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введіть Email")]
        [EmailAddress(ErrorMessage = "Невірний формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, ErrorMessage = "Пароль повинен містити мінімум {2} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Виберіть тип користувача")]
        [Display(Name = "Тип користувача")]
        public string UserType { get; set; }

        [Display(Name = "Спеціалізація")]
        public string Specialization { get; set; }

        [Display(Name = "Дата народження")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}