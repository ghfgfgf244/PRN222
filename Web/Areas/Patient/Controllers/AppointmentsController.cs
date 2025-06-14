using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using DataAccessObjects;

namespace Web.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class AppointmentsController : Controller
    {
        private readonly AppointmentsDbContext _context;

        public AppointmentsController(AppointmentsDbContext context)
        {
            _context = context;
        }

        // GET: Patient/Appointments
        public async Task<IActionResult> Index()
        {
            var appointmentsDbContext = _context.Appointments.Include(a => a.Doctor).Include(a => a.Patient).Include(a => a.Specialty);
            return View(await appointmentsDbContext.ToListAsync());
        }

        // GET: Patient/Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Specialty)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Patient/Appointments/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "Avatar");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name");
            return View();
        }

        // POST: Patient/Appointments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PatientId,DoctorId,SpecialtyId,ExamMethod,AppointmentDate,TimeSlot,Status,PaymentStatus,CreatedAt")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "Avatar", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointment.PatientId);
            ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name", appointment.SpecialtyId);
            return View(appointment);
        }

        // GET: Patient/Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "Avatar", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointment.PatientId);
            ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name", appointment.SpecialtyId);
            return View(appointment);
        }

        // POST: Patient/Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,PatientId,DoctorId,SpecialtyId,ExamMethod,AppointmentDate,TimeSlot,Status,PaymentStatus,CreatedAt")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["DoctorId"] = new SelectList(_context.Users, "UserId", "Avatar", appointment.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointment.PatientId);
            ViewData["SpecialtyId"] = new SelectList(_context.DoctorSpecialties, "SpecialtyId", "Name", appointment.SpecialtyId);
            return View(appointment);
        }

        // GET: Patient/Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Specialty)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Patient/Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
