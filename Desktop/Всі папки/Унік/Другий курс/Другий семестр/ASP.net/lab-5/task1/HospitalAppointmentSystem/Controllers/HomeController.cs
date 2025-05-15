using Microsoft.AspNetCore.Mvc;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // Додано цей рядок

namespace HospitalAppointmentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                DoctorsCount = await _context.Doctors.CountAsync(),
                PatientsCount = await _context.Patients.CountAsync(),
                MedicalRecordsCount = await _context.MedicalRecords.CountAsync(),
                ScheduledAppointments = await _context.Appointments
                    .CountAsync(a => a.Status == AppointmentStatus.Scheduled),
                TotalAppointments = await _context.Appointments.CountAsync(),
                UpcomingAppointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .Where(a => a.AppointmentDateTime.Date >= DateTime.Today)
                    .OrderBy(a => a.AppointmentDateTime)
                    .ToListAsync()
            };

            if (User.Identity?.IsAuthenticated ?? false)
            {
                if (User.IsInRole("Doctor"))
                {
                    var doctorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    viewModel.UpcomingAppointments = viewModel.UpcomingAppointments
                        .Where(a => a.DoctorId == doctorId)
                        .ToList();
                }
                else
                {
                    var patientId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    viewModel.UpcomingAppointments = viewModel.UpcomingAppointments
                        .Where(a => a.PatientId == patientId)
                        .ToList();
                }
            }

            return View(viewModel);
        }
    }
}