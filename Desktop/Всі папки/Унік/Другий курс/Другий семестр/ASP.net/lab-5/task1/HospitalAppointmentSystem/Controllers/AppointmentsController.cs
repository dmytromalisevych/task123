using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;
using HospitalAppointmentSystem.Infrastructure;

namespace HospitalAppointmentSystem.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly FileSessionStorage _fileStorage;
        private const string APPOINTMENT_DRAFT_KEY = "AppointmentDraft";

        public AppointmentsController(
            AppDbContext context,
            ILogger<AppointmentsController> logger,
            FileSessionStorage fileStorage)
        {
            _context = context;
            _logger = logger;
            _fileStorage = fileStorage;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Doctor)
                    .Include(a => a.Patient)
                    .OrderByDescending(a => a.AppointmentDateTime)
                    .ToListAsync();

                var viewModel = new AppointmentListViewModel
                {
                    Appointments = appointments,
                    TotalAppointments = appointments.Count,
                    ScheduledAppointments = appointments.Count(a => a.Status == AppointmentStatus.Scheduled),
                    CompletedAppointments = appointments.Count(a => a.Status == AppointmentStatus.Completed)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Index: {ex.Message}");
                return View(new AppointmentListViewModel());
            }
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var draft = _fileStorage.Load<AppointmentDraft>(HttpContext.Session.Id) ?? 
                           HttpContext.Session.GetJson<AppointmentDraft>(APPOINTMENT_DRAFT_KEY);

                var viewModel = new AppointmentCreateViewModel
                {
                    PatientId = draft?.PatientId ?? 0,
                    DoctorId = draft?.DoctorId ?? 0,
                    AppointmentDateTime = draft?.AppointmentDateTime ?? DateTime.Now.Date.AddDays(1).AddHours(9),
                    Notes = draft?.Notes,
                    Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync(),
                    Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create GET: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public IActionResult SaveDraft([FromForm] AppointmentCreateViewModel model)
        {
            try
            {
                var draft = new AppointmentDraft
                {
                    PatientId = model.PatientId,
                    DoctorId = model.DoctorId,
                    AppointmentDateTime = model.AppointmentDateTime,
                    Notes = model.Notes,
                    CreatedAt = DateTime.UtcNow
                };
                
                HttpContext.Session.SetJson(APPOINTMENT_DRAFT_KEY, draft);
                _fileStorage.Save(HttpContext.Session.Id, draft);

                return Json(new { success = true, message = "Чернетку збережено" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error saving draft: {ex.Message}");
                return Json(new { success = false, message = "Помилка збереження чернетки" });
            }
        }

        public async Task<IActionResult> ResumeDraft()
        {
            try
            {
                var draft = _fileStorage.Load<AppointmentDraft>(HttpContext.Session.Id) ??
                           HttpContext.Session.GetJson<AppointmentDraft>(APPOINTMENT_DRAFT_KEY);

                if (draft == null)
                {
                    return RedirectToAction(nameof(Create));
                }

                var viewModel = new AppointmentCreateViewModel
                {
                    PatientId = draft.PatientId ?? 0,
                    DoctorId = draft.DoctorId ?? 0,
                    AppointmentDateTime = draft.AppointmentDateTime ?? DateTime.Now.Date.AddDays(1).AddHours(9),
                    Notes = draft.Notes,
                    Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync(),
                    Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync()
                };
                
                HttpContext.Session.SetJson(APPOINTMENT_DRAFT_KEY, draft);

                return View("Create", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resuming draft: {ex.Message}");
                return RedirectToAction(nameof(Create));
            }
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DoctorId,AppointmentDateTime,Notes")] AppointmentCreateViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var appointmentTime = model.AppointmentDateTime.TimeOfDay;
                    if (appointmentTime < new TimeSpan(9, 0, 0) || appointmentTime >= new TimeSpan(18, 0, 0))
                    {
                        ModelState.AddModelError("AppointmentDateTime", "Час прийому має бути між 9:00 та 18:00");
                        model.Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync();
                        model.Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync();
                        return View(model);
                    }

                    var existingAppointment = await _context.Appointments
                        .AnyAsync(a => a.DoctorId == model.DoctorId &&
                                     a.AppointmentDateTime == model.AppointmentDateTime &&
                                     a.Status == AppointmentStatus.Scheduled);

                    if (existingAppointment)
                    {
                        ModelState.AddModelError("", "На цей час вже є запис до даного лікаря");
                        model.Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync();
                        model.Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync();
                        return View(model);
                    }

                    var appointment = new Appointment
                    {
                        DoctorId = model.DoctorId,
                        PatientId = model.PatientId,
                        AppointmentDateTime = model.AppointmentDateTime,
                        Notes = model.Notes,
                        Status = AppointmentStatus.Scheduled
                    };

                    _context.Add(appointment);
                    await _context.SaveChangesAsync();
                    
                    ClearDraft();

                    return RedirectToAction(nameof(Index));
                }

                model.Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync();
                model.Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create POST: {ex.Message}");
                ModelState.AddModelError("", "Виникла помилка при створенні призначення");
                model.Doctors = await _context.Doctors.OrderBy(d => d.LastName).ToListAsync();
                model.Patients = await _context.Patients.OrderBy(p => p.LastName).ToListAsync();
                return View(model);
            }
        }
        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment != null)
                {
                    _context.Appointments.Remove(appointment);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Deleted appointment: {id}");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting appointment {id}: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
        private void ClearDraft()
        {
            var sessionId = HttpContext.Session.Id;
            HttpContext.Session.Remove(APPOINTMENT_DRAFT_KEY);
            _fileStorage.Delete(sessionId);
        }
    }
}