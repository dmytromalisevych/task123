using HospitalAppointmentSystem.Core.Models.Base;

namespace HospitalAppointmentSystem.Core.Models
{
    public class Doctor : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Specialization { get; set; }
        public string PhoneNumber { get; set; }
        
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; }
    }
}