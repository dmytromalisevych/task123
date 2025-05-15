using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalAppointmentSystem.Models;
using HospitalAppointmentSystem.Models.ViewModels;

namespace HospitalAppointmentSystem.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly int pageSize = 10;

        public MedicalRecordsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MedicalRecords
        public async Task<IActionResult> Index(string searchString, int page = 1)
        {
            var query = _context.MedicalRecords
                .Include(m => m.Patient)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(m => 
                    m.Diagnosis.Contains(searchString) || 
                    m.Treatment.Contains(searchString) ||
                    m.Patient.FirstName.Contains(searchString) ||
                    m.Patient.LastName.Contains(searchString));
            }

            var totalRecords = await query.CountAsync();

            var medicalRecords = await query
                .OrderByDescending(m => m.RecordDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new MedicalRecordListViewModel
            {
                MedicalRecords = medicalRecords,
                SearchString = searchString,
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = totalRecords
                }
            };

            return View(viewModel);
        }

        // GET: MedicalRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalRecordId == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // GET: MedicalRecords/Create
        public IActionResult Create()
        {
            var viewModel = new MedicalRecordCreateViewModel
            {
                RecordDate = DateTime.Today,
                Patients = _context.Patients.OrderBy(p => p.LastName).ToList()
            };
            return View(viewModel);
        }

        // POST: MedicalRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicalRecordCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var medicalRecord = new MedicalRecord
                {
                    Diagnosis = viewModel.Diagnosis,
                    Treatment = viewModel.Treatment,
                    RecordDate = viewModel.RecordDate,
                    PatientId = viewModel.PatientId
                };

                _context.Add(medicalRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            viewModel.Patients = _context.Patients.OrderBy(p => p.LastName).ToList();
            return View(viewModel);
        }

        // GET: MedicalRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            var viewModel = new MedicalRecordCreateViewModel
            {
                Diagnosis = medicalRecord.Diagnosis,
                Treatment = medicalRecord.Treatment,
                RecordDate = medicalRecord.RecordDate,
                PatientId = medicalRecord.PatientId,
                Patients = _context.Patients.OrderBy(p => p.LastName).ToList()
            };

            return View(viewModel);
        }

        // POST: MedicalRecords/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicalRecordCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var medicalRecord = await _context.MedicalRecords.FindAsync(id);
                    if (medicalRecord == null)
                    {
                        return NotFound();
                    }

                    medicalRecord.Diagnosis = viewModel.Diagnosis;
                    medicalRecord.Treatment = viewModel.Treatment;
                    medicalRecord.RecordDate = viewModel.RecordDate;
                    medicalRecord.PatientId = viewModel.PatientId;

                    _context.Update(medicalRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            viewModel.Patients = _context.Patients.OrderBy(p => p.LastName).ToList();
            return View(viewModel);
        }

        // GET: MedicalRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalRecordId == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // POST: MedicalRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.MedicalRecordId == id);
        }
    }
}