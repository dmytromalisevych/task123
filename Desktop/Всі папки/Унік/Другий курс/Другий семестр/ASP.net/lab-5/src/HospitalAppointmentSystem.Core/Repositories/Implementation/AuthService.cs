using HospitalAppointmentSystem.Core.Services.Interfaces;
using HospitalAppointmentSystem.Shared.Models.Requests;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;

namespace HospitalAppointmentSystem.Core.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<(string Id, string Email, string Role)?> ValidateUserAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return null;

            var isValidPassword = await _userManager.CheckPasswordAsync(user, password);
            if (!isValidPassword)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "User";

            return (user.Id, user.Email, role);
        }

        public async Task<AuthResult> RegisterUserAsync(RegisterRequest request)
        {
            var user = new IdentityUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            await _userManager.AddToRoleAsync(user, request.UserType);

            return new AuthResult
            {
                Succeeded = true,
                UserId = user.Id
            };
        }
    }
}