using System.ComponentModel.DataAnnotations;
namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class MedicalRecordListViewModel
    {
        public IEnumerable<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public PagingInfo PagingInfo { get; set; } = new();
        public string SearchString { get; set; } = string.Empty;
    }

    public class MedicalRecordCreateViewModel
    {
        [Required(ErrorMessage = "Будь ласка, введіть діагноз")]
        [Display(Name = "Діагноз")]
        [MaxLength(200)]
        public string Diagnosis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, введіть деталі лікування")]
        [Display(Name = "Лікування")]
        [MaxLength(500)]
        public string Treatment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Будь ласка, введіть дату запису")]
        [Display(Name = "Дата запису")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        [Required(ErrorMessage = "Виберіть пацієнта")]
        [Display(Name = "Пацієнт")]
        public int PatientId { get; set; }

        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();
    }
}