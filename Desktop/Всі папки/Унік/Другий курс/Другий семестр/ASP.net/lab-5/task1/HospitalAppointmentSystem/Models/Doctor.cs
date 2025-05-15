using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Поле Ім'я є обов'язковим")]
        [Display(Name = "Ім'я")]
        [StringLength(50, ErrorMessage = "Ім'я не може бути довшим за 50 символів")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Поле Прізвище є обов'язковим")]
        [Display(Name = "Прізвище")]
        [StringLength(50, ErrorMessage = "Прізвище не може бути довшим за 50 символів")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Поле Спеціалізація є обов'язковим")]
        [Display(Name = "Спеціалізація")]
        [StringLength(100, ErrorMessage = "Спеціалізація не може бути довшою за 100 символів")]
        public string Specialization { get; set; }
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName => $"{LastName} {FirstName}";
        
        public virtual ICollection<Appointment>? Appointments { get; set; }
    }
}