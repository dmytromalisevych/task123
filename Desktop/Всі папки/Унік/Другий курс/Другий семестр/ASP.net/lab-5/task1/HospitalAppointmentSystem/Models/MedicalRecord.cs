using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalAppointmentSystem.Models
{
    public class MedicalRecord
    {
        public int MedicalRecordId { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть діагноз")]
        [Display(Name = "Діагноз")]
        [MaxLength(200, ErrorMessage = "Діагноз не може перевищувати 200 символів")]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, введіть деталі лікування")]
        [Display(Name = "Лікування")]
        [MaxLength(500, ErrorMessage = "Деталі лікування не можуть перевищувати 500 символів")]
        public string Treatment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, введіть дату запису")]
        [Display(Name = "Дата запису")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        [Required(ErrorMessage = "Необхідно вказати пацієнта")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}