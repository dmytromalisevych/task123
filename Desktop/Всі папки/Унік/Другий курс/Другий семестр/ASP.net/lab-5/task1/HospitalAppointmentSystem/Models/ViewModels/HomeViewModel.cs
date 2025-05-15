namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class HomeViewModel
    {
        public int DoctorsCount { get; set; }
        public int PatientsCount { get; set; }
        public int ScheduledAppointments { get; set; }
        public int TotalAppointments { get; set; }
        public int MedicalRecordsCount { get; set; }
        public IEnumerable<Appointment> UpcomingAppointments { get; set; }
        public Dictionary<string, int> DoctorsBySpecialization { get; set; }

        public HomeViewModel()
        {
            UpcomingAppointments = new List<Appointment>();
            DoctorsBySpecialization = new Dictionary<string, int>();
        }
    }
}