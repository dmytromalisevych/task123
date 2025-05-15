namespace HospitalAppointmentSystem.Core.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } 
        public string Description { get; set; }
        
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}