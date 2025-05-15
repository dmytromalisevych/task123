using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;


namespace HospitalAppointmentSystem.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DoctorsController> _logger;
        private readonly int _pageSize = 10;

        public DoctorsController(AppDbContext context, ILogger<DoctorsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Doctors
        public async Task<IActionResult> Index(string searchString, string specialization, int page = 1)
        {
            try
            {
                var doctorsQuery = _context.Doctors
                    .Include(d => d.Appointments)
                    .AsQueryable();
                
                if (!string.IsNullOrEmpty(searchString))
                {
                    doctorsQuery = doctorsQuery.Where(d =>
                        d.FirstName.Contains(searchString) ||
                        d.LastName.Contains(searchString) ||
                        d.Specialization.Contains(searchString));
                }
                
                if (!string.IsNullOrEmpty(specialization))
                {
                    doctorsQuery = doctorsQuery.Where(d => d.Specialization == specialization);
                }
                
                var totalItems = await doctorsQuery.CountAsync();
                
                var doctors = await doctorsQuery
                    .OrderBy(d => d.LastName)
                    .Skip((page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToListAsync();
                
                var specializations = await _context.Doctors
                    .Select(d => d.Specialization)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                var viewModel = new DoctorsListViewModel
                {
                    Doctors = doctors,
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = _pageSize,
                        TotalItems = totalItems
                    },
                    CurrentSpecialization = specialization,
                    SearchString = searchString,
                    Specializations = specializations
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при отриманні списку лікарів: {ex.Message}");
                TempData["Error"] = "Виникла помилка при завантаженні даних";
                return View(new DoctorsListViewModel());
            }
        }

        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Appointments)
                    .ThenInclude(a => a.Patient)
                    .FirstOrDefaultAsync(d => d.DoctorId == id);

                if (doctor == null)
                {
                    return NotFound();
                }
                
                var activeAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Where(a => a.DoctorId == id && 
                               a.Status == AppointmentStatus.Scheduled && 
                               a.AppointmentDateTime > DateTime.Now)
                    .OrderBy(a => a.AppointmentDateTime)
                    .ToListAsync();

                ViewBag.ActiveAppointments = activeAppointments;

                return View(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при отриманні деталей лікаря: {ex.Message}");
                TempData["Error"] = "Виникла помилка при завантаженні даних";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Specialization")] Doctor doctor)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = string.Join("; ", ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));
                    _logger.LogWarning($"Невалідна модель при створенні лікаря: {errors}");
                    return View(doctor);
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        _context.Doctors.Add(doctor);
                        await _context.SaveChangesAsync();
                        
                        await transaction.CommitAsync();
                
                        _logger.LogInformation($"Лікаря успішно додано: {doctor.FullName}");
                        TempData["Success"] = $"Лікаря {doctor.FullName} успішно додано";
                
                        return RedirectToAction(nameof(Index));
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw; 
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при створенні лікаря: {ex.Message}");
                ModelState.AddModelError("", "Виникла помилка при створенні лікаря. Спробуйте ще раз.");
                return View(doctor);
            }
        }
        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Appointments)
                    .FirstOrDefaultAsync(d => d.DoctorId == id);

                if (doctor == null)
                {
                    return NotFound();
                }

                return View(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при завантаженні даних лікаря для редагування: {ex.Message}");
                TempData["Error"] = "Виникла помилка при завантаженні даних";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,FirstName,LastName,Specialization")] Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var existingDoctor = await _context.Doctors
                            .Include(d => d.Appointments)
                            .FirstOrDefaultAsync(d => d.DoctorId == id);

                        if (existingDoctor == null)
                        {
                            return NotFound();
                        }
                        
                        existingDoctor.FirstName = doctor.FirstName;
                        existingDoctor.LastName = doctor.LastName;
                        existingDoctor.Specialization = doctor.Specialization;

                        _context.Update(existingDoctor);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["Success"] = "Дані лікаря успішно оновлено";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
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
                    await transaction.RollbackAsync();
                    _logger.LogError($"Помилка при оновленні даних лікаря: {ex.Message}");
                    ModelState.AddModelError("", "Виникла помилка при оновленні даних лікаря");
                }
            }
            return View(doctor);
        }

        // GET: Doctors/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var doctor = await _context.Doctors
                    .Include(d => d.Appointments)
                    .FirstOrDefaultAsync(d => d.DoctorId == id);

                if (doctor == null)
                {
                    return NotFound();
                }
                
                if (doctor.Appointments.Any(a => a.Status == AppointmentStatus.Scheduled))
                {
                    TempData["Error"] = "Неможливо видалити лікаря з активними призначеннями";
                    return RedirectToAction(nameof(Index));
                }

                return View(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Помилка при завантаженні даних лікаря для видалення: {ex.Message}");
                TempData["Error"] = "Виникла помилка при завантаженні даних";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var doctor = await _context.Doctors
                        .Include(d => d.Appointments)
                        .FirstOrDefaultAsync(d => d.DoctorId == id);

                    if (doctor == null)
                    {
                        return NotFound();
                    }
                    
                    if (doctor.Appointments.Any(a => a.Status == AppointmentStatus.Scheduled))
                    {
                        TempData["Error"] = "Неможливо видалити лікаря з активними призначеннями";
                        return RedirectToAction(nameof(Index));
                    }
                    
                    var appointments = await _context.Appointments
                        .Where(a => a.DoctorId == id)
                        .ToListAsync();

                    _context.Appointments.RemoveRange(appointments);
                    
                    _context.Doctors.Remove(doctor);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Лікаря успішно видалено";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError($"Помилка при видаленні лікаря: {ex.Message}");
                    TempData["Error"] = "Виникла помилка при видаленні лікаря";
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}