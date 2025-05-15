namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class PatientListViewModel
    {
        public IEnumerable<Patient> Patients { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string SearchString { get; set; }
    }
}