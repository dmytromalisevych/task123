using System;
using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public DateTime AppointmentDateTime { get; set; }

        [Required]
        public AppointmentStatus Status { get; set; }

        public string Notes { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual Patient Patient { get; set; }
    }

    public enum AppointmentStatus
    {
        Scheduled,
        Completed,
        Cancelled
    }
}