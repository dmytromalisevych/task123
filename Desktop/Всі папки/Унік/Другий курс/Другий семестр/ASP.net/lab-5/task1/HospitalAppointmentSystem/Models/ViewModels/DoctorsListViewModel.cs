namespace HospitalAppointmentSystem.Models.ViewModels
{
    public class DoctorsListViewModel
    {
        public IEnumerable<Doctor> Doctors { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentSpecialization { get; set; }
        public string SearchString { get; set; }
        public IEnumerable<string> Specializations { get; set; }
    }
}