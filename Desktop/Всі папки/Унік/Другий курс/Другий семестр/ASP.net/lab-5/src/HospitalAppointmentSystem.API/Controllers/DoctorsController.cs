using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HospitalAppointmentSystem.Core.Models;
using HospitalAppointmentSystem.Core.Repositories.Interfaces;

namespace HospitalAppointmentSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : ControllerBase
    {
        private readonly IRepository<Doctor> _doctorRepository;

        public DoctorsController(IRepository<Doctor> doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            var doctors = await _doctorRepository.GetAllAsync();
            return Ok(doctors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctor(int id)
        {
            var doctor = await _doctorRepository.GetByIdAsync(id);
            if (doctor == null)
                return NotFound();
            
            return Ok(doctor);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Doctor>> CreateDoctor(Doctor doctor)
        {
            await _doctorRepository.AddAsync(doctor);
            return CreatedAtAction(nameof(GetDoctor), new { id = doctor.Id }, doctor);
        }
    }
}