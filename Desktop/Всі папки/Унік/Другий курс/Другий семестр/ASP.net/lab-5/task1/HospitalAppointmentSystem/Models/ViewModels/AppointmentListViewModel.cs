using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class AppointmentListViewModel
    {
        public IEnumerable<Appointment> Appointments { get; set; }
        public string SearchString { get; set; }
        public string CurrentStatus { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public int TotalAppointments { get; set; }
        public int ScheduledAppointments { get; set; }
        public int CompletedAppointments { get; set; }
    }

    public class AppointmentCreateViewModel
    {
        [Required(ErrorMessage = "Виберіть пацієнта")]
        [Display(Name = "Пацієнт")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Виберіть лікаря")]
        [Display(Name = "Лікар")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Вкажіть дату та час")]
        [Display(Name = "Дата та час")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDateTime { get; set; }

        [Display(Name = "Нотатки")]
        public string Notes { get; set; }
        
        public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();
        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();
    }
}