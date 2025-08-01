using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BusinessObjects;
using Services;
using System.Security.Claims;

namespace Web_.Pages.Appointments
{
    public class EditModel : PageModel
    {
        private readonly IExternalIntegrationService _exContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IDoctorServices _doctorContext;
        private readonly IAppointmentServices _appointmentContext;
        private readonly IPatientServices _patientContext;
        private readonly IConfiguration _configuration;

        public EditModel(IPatientServices patientServices, IAppointmentServices appointmentServices, IDoctorServices doctorServices, IConfiguration configuration, IWebHostEnvironment environment)
        {
            _exContext = new ExternalIntegrationService(configuration);
            this._environment = environment;
            _doctorContext = doctorServices;
            _appointmentContext = appointmentServices;
            _patientContext = patientServices;
            _configuration = configuration;

        }

        [BindProperty]
        public Appointment Appointment { get; set; } = default!;
        [BindProperty]
        public Patient Patient { get; set; } = default!;
        [BindProperty]
        public int? UserId { get; set; }
        [BindProperty]
        public int? PatientId { get; set; }
        [BindProperty]
        public int? SelectedSpecialtyId { get; set; }
        [BindProperty]
        public int? SelectedSlotId { get; set; }
        [BindProperty]
        public int? SelectedMethodId { get; set; }
        [BindProperty]
        public int? SelectedDoctorId { get; set; }
        public List<SelectListItem> SlotOptions { get; set; } = new();
        public List<SelectListItem> SpecialtyOptions { get; set; } = new();
        public List<SelectListItem> MethodOptions { get; set; } = new();
        public List<SelectListItem> DoctorOptions { get; set; } = new();
        public string GeminiApiKey { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            GeminiApiKey = _configuration["Gemini:ApiKey"];

            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _appointmentContext.GetAppointmentByIdAsync(id.Value);
            if (appointment == null)
            {
                return NotFound();
            }
            Appointment = appointment;

            var patient = await _patientContext.GetPatientByIdAsync(appointment.PatientId.Value);
            if (patient == null)
            {
                return NotFound();
            }
            Patient = patient;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Account/AccessDenied");
            }
            UserId = userId;

            if (Patient.RegisteredBy != UserId)
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            await LoadInitialDropdowns();

            SelectedSpecialtyId = Appointment.SpecialtyId;
            SelectedSlotId = Appointment.SlotId;
            SelectedMethodId = Appointment.MethodId;
            SelectedDoctorId = Appointment.DoctorId;

            if (Appointment.SpecialtyId != null)
            {
                var methods = await _appointmentContext.GetMethodsBySpecialtyId(Appointment.SpecialtyId.Value);
                MethodOptions = methods.Select(m => new SelectListItem
                {
                    Value = m.MethodId.ToString(),
                    Text = m.Name
                }).ToList();
            }

            if (Appointment.SpecialtyId != null && Appointment.SlotId != null && Appointment.AppointmentDate != null)
            {
                var doctors = await _appointmentContext.GetAvailableDoctors(
                    Appointment.SpecialtyId.Value,
                    Appointment.SlotId.Value,
                    Appointment.AppointmentDate
                );
                DoctorOptions = doctors.Select(d => new SelectListItem
                {
                    Value = d.UserId.ToString(),
                    Text = d.FullName
                }).ToList();
            }

            if (!DoctorOptions.Any(d => d.Value == Appointment.DoctorId.ToString()))
            {
                var doctor = await _doctorContext.GetDoctorByIdAsync(Appointment.DoctorId ?? 0);
                if (doctor != null)
                {
                    DoctorOptions.Add(new SelectListItem
                    {
                        Value = doctor.UserId.ToString(),
                        Text = doctor.FullName + " (hiện tại)"
                    });
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        Console.WriteLine($"Model error in {modelState.Key}: {error.ErrorMessage}");
                    }
                }

                await LoadInitialDropdowns();

                if (Appointment.SpecialtyId != null)
                {
                    var methods = await _appointmentContext.GetMethodsBySpecialtyId(Appointment.SpecialtyId.Value);
                    MethodOptions = methods.Select(m => new SelectListItem
                    {
                        Value = m.MethodId.ToString(),
                        Text = m.Name
                    }).ToList();
                }

                if (Appointment.SpecialtyId != null && Appointment.SlotId != null && Appointment.AppointmentDate != null)
                {
                    var doctors = await _appointmentContext.GetAvailableDoctors(
                        Appointment.SpecialtyId.Value,
                        Appointment.SlotId.Value,
                        Appointment.AppointmentDate
                    );

                    DoctorOptions = doctors.Select(d => new SelectListItem
                    {
                        Value = d.UserId.ToString(),
                        Text = d.FullName
                    }).ToList();

                    // ✅ Nếu doctor hiện tại không có trong danh sách, thêm vào
                    if (!DoctorOptions.Any(d => d.Value == SelectedDoctorId.ToString()))
                    {
                        var doctor = await _doctorContext.GetDoctorByIdAsync(SelectedDoctorId ?? 0);
                        if (doctor != null)
                        {
                            DoctorOptions.Add(new SelectListItem
                            {
                                Value = doctor.UserId.ToString(),
                                Text = doctor.FullName + " (hiện tại)"
                            });
                        }
                    }
                }

                return Page();
            }
            Appointment.AppointmentDate = Appointment.AppointmentDate;
            Appointment.SpecialtyId = SelectedSpecialtyId;
            Appointment.SlotId = SelectedSlotId;
            Appointment.MethodId = SelectedMethodId;
            Appointment.DoctorId = SelectedDoctorId;
            Appointment.PatientId = Patient.PatientId;

            await _patientContext.UpdatePatientAsync(Patient);
            Console.WriteLine($"FullName: {Patient.FullName}");

            await _appointmentContext.UpdateAppointmentAsync(Appointment);
            Console.WriteLine($"SlotId: {Appointment.SlotId}, SpecialtyId: {Appointment.SpecialtyId}, MethodId: {Appointment.MethodId}, DoctorId: {Appointment.DoctorId}");

            return RedirectToPage("./Index");
        }

        private async Task LoadInitialDropdowns()
        {
            SpecialtyOptions = (await _doctorContext.GetAllSpecialties())
                .Select(s => new SelectListItem
                {
                    Value = s.SpecialtyId.ToString(),
                    Text = s.Name
                }).ToList();

            SlotOptions = (await _appointmentContext.GetAllSlots())
                .Select(s => new SelectListItem
                {
                    Value = s.SlotId.ToString(),
                    Text = $"{s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm}"
                }).ToList();
        }
        public async Task<JsonResult> OnGetMethodsBySpecialty(int specialtyId)
        {
            var methods = await _appointmentContext.GetMethodsBySpecialtyId(specialtyId);
            var result = methods.Select(m => new SelectListItem
            {
                Value = m.MethodId.ToString(),
                Text = m.Name
            }).ToList();

            return new JsonResult(result);
        }
        public async Task<JsonResult> OnGetDoctorsByFilters(int specialtyId, int slotId, string date)
        {
            Console.WriteLine("voooo");
            if (!DateOnly.TryParse(date, out var parsedDate))
            {
                return new JsonResult(new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = "",
                Text = "Ngày không hợp lệ"
            }
        });
            }

            var doctors = await _appointmentContext.GetAvailableDoctors(specialtyId, slotId, parsedDate);

            if (doctors == null || !doctors.Any())
            {
                return new JsonResult(new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = "",
                Text = "Không có bác sĩ phù hợp trong slot này"
            }
        });
            }

            var result = doctors.Select(d => new SelectListItem
            {
                Value = d.UserId.ToString(),
                Text = d.FullName
            }).ToList();

            return new JsonResult(result);
        }
    }
}