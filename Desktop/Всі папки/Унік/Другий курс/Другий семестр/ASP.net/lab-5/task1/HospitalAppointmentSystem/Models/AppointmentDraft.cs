namespace HospitalAppointmentSystem.Models
{
    public class AppointmentDraft
    {
        public int Id { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime? AppointmentDateTime { get; set; }
        public string? Notes { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        
        public Patient? Patient { get; set; }
        public Doctor? Doctor { get; set; }
    }
}