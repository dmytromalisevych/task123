namespace HospitalAppointmentSystem.API.Models.Responses
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }
        public string UserType { get; set; }
    }
}