using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models
{
    public class Patient
    {
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Введіть прізвище")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Введіть ім'я")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Введіть дату народження")]
        [Display(Name = "Дата народження")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public string FullName => $"{LastName} {FirstName}";
        public virtual ICollection<Appointment>? Appointments { get; set; }
        public virtual ICollection<MedicalRecord>? MedicalRecords { get; set; }
    }
}