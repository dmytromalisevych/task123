using System.ComponentModel.DataAnnotations;

namespace HospitalAppointmentSystem.Shared.Models.Requests
{
    public class RegisterRequest
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string UserType { get; set; }

        public string? Specialization { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}