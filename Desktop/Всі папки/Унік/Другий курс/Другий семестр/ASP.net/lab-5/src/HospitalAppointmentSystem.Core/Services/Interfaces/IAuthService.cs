using HospitalAppointmentSystem.Shared.Models.Requests;

namespace HospitalAppointmentSystem.Core.Services.Interfaces
{
    public class AuthResult
    {
        public bool Succeeded { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string UserId { get; set; }
    }

    public interface IAuthService
    {
        Task<(string Id, string Email, string Role)?> ValidateUserAsync(string email, string password);
        Task<AuthResult> RegisterUserAsync(RegisterRequest request);
    }
}