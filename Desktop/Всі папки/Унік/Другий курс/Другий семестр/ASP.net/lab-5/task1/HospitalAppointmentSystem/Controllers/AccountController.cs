using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;

namespace HospitalAppointmentSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doctor = await _context.Doctors
                    .FirstOrDefaultAsync(d => d.Email == model.Email);
                
                if (doctor != null)
                {
                    string hashedPassword = HashPassword(model.Password);
                    if (doctor.PasswordHash == hashedPassword)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, doctor.Email),
                            new Claim(ClaimTypes.Role, "Doctor"),
                            new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString())
                        };

                        await SignInWithClaims(claims);
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                var patient = await _context.Patients
                    .FirstOrDefaultAsync(p => p.Email == model.Email);
                    
                if (patient != null)
                {
                    string hashedPassword = HashPassword(model.Password);
                    if (patient.PasswordHash == hashedPassword)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, patient.Email),
                            new Claim(ClaimTypes.Role, "Patient"),
                            new Claim(ClaimTypes.NameIdentifier, patient.PatientId.ToString())
                        };

                        await SignInWithClaims(claims);
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Невірний email або пароль");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterViewModel
            {
                UserType = "Patient" 
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool emailExists = await _context.Doctors.AnyAsync(d => d.Email == model.Email) ||
                                 await _context.Patients.AnyAsync(p => p.Email == model.Email);

                if (emailExists)
                {
                    ModelState.AddModelError("Email", "Цей email вже зареєстрований");
                    return View(model);
                }

                string hashedPassword = HashPassword(model.Password);

                if (model.UserType == "Doctor")
                {
                    if (string.IsNullOrEmpty(model.Specialization))
                    {
                        ModelState.AddModelError("Specialization", "Вкажіть спеціалізацію");
                        return View(model);
                    }

                    var doctor = new Doctor
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PasswordHash = hashedPassword,
                        Specialization = model.Specialization
                    };

                    _context.Doctors.Add(doctor);
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, doctor.Email),
                        new Claim(ClaimTypes.Role, "Doctor"),
                        new Claim(ClaimTypes.NameIdentifier, doctor.DoctorId.ToString())
                    };

                    await SignInWithClaims(claims);
                }
                else if (model.UserType == "Patient")
                {
                    if (!model.DateOfBirth.HasValue)
                    {
                        ModelState.AddModelError("DateOfBirth", "Вкажіть дату народження");
                        return View(model);
                    }

                    var patient = new Patient
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PasswordHash = hashedPassword,
                        DateOfBirth = model.DateOfBirth.Value
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, patient.Email),
                        new Claim(ClaimTypes.Role, "Patient"),
                        new Claim(ClaimTypes.NameIdentifier, patient.PatientId.ToString())
                    };

                    await SignInWithClaims(claims);
                }

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        [HttpGet] 
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Identity.Application");
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInWithClaims(List<Claim> claims)
        {
            var claimsIdentity = new ClaimsIdentity(claims, "Identity.Application");
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                "Identity.Application",
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}