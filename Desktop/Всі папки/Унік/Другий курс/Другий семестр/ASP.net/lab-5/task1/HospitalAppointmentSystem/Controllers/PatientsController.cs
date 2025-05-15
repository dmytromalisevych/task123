using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;


namespace HospitalAppointmentSystem.Controllers
{
    public class PatientsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PatientsController> _logger;
        private readonly int _pageSize = 10;

        public PatientsController(AppDbContext context, ILogger<PatientsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            try
            {
                var patientsQuery = _context.Patients
                    .Include(p => p.Appointments)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(searchString))
                {
                    patientsQuery = patientsQuery.Where(p =>
                        p.FirstName.Contains(searchString) ||
                        p.LastName.Contains(searchString));
                }

                var totalItems = await patientsQuery.CountAsync();
                var patients = await patientsQuery
                    .OrderBy(p => p.LastName)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();

                var viewModel = new PatientListViewModel
                {
                    Patients = patients,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = _pageSize,
                        TotalItems = totalItems
                    },
                    SearchString = searchString
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting patients list: {ex.Message}");
                TempData["Error"] = "Помилка при завантаженні списку пацієнтів";
                return View(new PatientListViewModel());
            }
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View(new Patient { DateOfBirth = DateTime.Today });
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Patient patient)
        {
            try
            {
                _logger.LogInformation($"Attempting to create patient: {patient.FirstName} {patient.LastName}");

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning($"Invalid model state: {errors}");

                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogWarning($"Validation error: {error.ErrorMessage}");
                    }

                    return View(patient);
                }

                _context.Add(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully created patient with ID: {patient.PatientId}");
                TempData["Success"] = "Пацієнта успішно додано";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating patient: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", "Помилка при створенні пацієнта. Спробуйте ще раз.");
                return View(patient);
            }
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit attempted with null ID");
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                _logger.LogWarning($"Patient with ID {id} not found");
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.PatientId)
            {
                _logger.LogWarning($"ID mismatch: {id} vs {patient.PatientId}");
                return NotFound();
            }

            try
            {
                _logger.LogInformation($"Attempting to update patient {id}");

                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    _logger.LogWarning($"Invalid model state: {errors}");
                    return View(patient);
                }

                _context.Update(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Successfully updated patient {id}");
                TempData["Success"] = "Дані пацієнта оновлено";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError($"Concurrency error updating patient {id}: {ex.Message}");
                if (!PatientExists(patient.PatientId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating patient {id}: {ex.Message}");
                ModelState.AddModelError("", "Помилка при оновленні даних пацієнта");
                return View(patient);
            }
        }

        // GET: Patients/Details/5
public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    try
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
            .ThenInclude(a => a.Doctor)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient == null)
        {
            return NotFound();
        }
        
        var activeAppointments = await _context.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == id && 
                       a.Status == AppointmentStatus.Scheduled && 
                       a.AppointmentDateTime > DateTime.Now)
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();

        ViewBag.ActiveAppointments = activeAppointments;

        return View(patient);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Помилка при отриманні деталей пацієнта: {ex.Message}");
        TempData["Error"] = "Виникла помилка при завантаженні даних";
        return RedirectToAction(nameof(Index));
    }
}

// GET: Patients/Delete/5
[HttpGet]
public async Task<IActionResult> Delete(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    try
    {
        var patient = await _context.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.PatientId == id);

        if (patient == null)
        {
            return NotFound();
        }
        
        if (patient.Appointments != null && 
            patient.Appointments.Any(a => a.Status == AppointmentStatus.Scheduled))
        {
            TempData["Error"] = "Неможливо видалити пацієнта з активними призначеннями";
            return RedirectToAction(nameof(Index));
        }

        return View(patient);
    }
    catch (Exception ex)
    {
        _logger.LogError($"Помилка при завантаженні даних пацієнта для видалення: {ex.Message}");
        TempData["Error"] = "Виникла помилка при завантаженні даних";
        return RedirectToAction(nameof(Index));
    }
}

// POST: Patients/Delete/5
[HttpPost, ActionName("Delete")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> DeleteConfirmed(int id)
{
    using (var transaction = await _context.Database.BeginTransactionAsync())
    {
        try
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }
            
            if (patient.Appointments != null && 
                patient.Appointments.Any(a => a.Status == AppointmentStatus.Scheduled))
            {
                TempData["Error"] = "Неможливо видалити пацієнта з активними призначеннями";
                return RedirectToAction(nameof(Index));
            }
            
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == id)
                .ToListAsync();

            _context.Appointments.RemoveRange(appointments);

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            _logger.LogInformation($"Пацієнта успішно видалено: {patient.FullName}");
            TempData["Success"] = "Пацієнта успішно видалено";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError($"Помилка при видаленні пацієнта: {ex.Message}");
            TempData["Error"] = "Виникла помилка при видаленні пацієнта";
            return RedirectToAction(nameof(Index));
        }
    }
}

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}