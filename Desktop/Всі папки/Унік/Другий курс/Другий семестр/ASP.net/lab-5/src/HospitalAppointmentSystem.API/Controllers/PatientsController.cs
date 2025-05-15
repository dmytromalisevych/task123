using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HospitalAppointmentSystem.Core.Models;
using HospitalAppointmentSystem.Core.Repositories.Interfaces;
using System.Security.Claims;

namespace HospitalAppointmentSystem.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IRepository<Patient> _patientRepository;

        public PatientsController(IRepository<Patient> patientRepository)
        {
            _patientRepository = patientRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            var patients = await _patientRepository.GetAllAsync();
            return Ok(patients);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                return NotFound();
            
            if (!User.IsInRole("Doctor") && User.FindFirst(ClaimTypes.NameIdentifier)?.Value != id.ToString())
                return Forbid();
            
            return Ok(patient);
        }
    }
}