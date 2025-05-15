using HospitalAppointmentSystem.Core.Models.Base;

namespace HospitalAppointmentSystem.Core.Models
{
    public class MedicalRecord : BaseEntity
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string Notes { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}