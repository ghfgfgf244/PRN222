using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObjects;
using Services;
using System.Security.Claims;
using System.Data;

namespace Web_.Pages.Appointments
{
    public class CreateModel : PageModel
    {
        private readonly IExternalIntegrationService _exContext;
        private readonly IWebHostEnvironment _environment;
        private readonly IDoctorServices _doctorContext;
        private readonly IAppointmentServices _appointmentContext;
        private readonly IPatientServices _patientContext;
        private readonly IConfiguration _configuration;

        public CreateModel(IPatientServices patientServices, IAppointmentServices appointmentServices, IDoctorServices doctorServices, IConfiguration configuration, IWebHostEnvironment environment)
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
        public string GeminiApiKey { get; set; }

        public async Task<IActionResult> OnGet()
        {
            GeminiApiKey = _configuration["Gemini:ApiKey"];

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return RedirectToPage("/Account/AccessDenied");
            }
            UserId = userId;
            await LoadInitialDropdowns();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _patientContext.AddPatientAsync(Patient);

            Appointment.PatientId = Patient.PatientId;
            Appointment.DoctorId = SelectedDoctorId.Value;
            Appointment.SlotId = SelectedSlotId.Value;
            Appointment.MethodId = SelectedMethodId.Value;
            Appointment.SpecialtyId = SelectedSpecialtyId.Value;
            Appointment.Status = "Pending";
            Appointment.CreatedAt = DateTime.Now;

            await _appointmentContext.CreateAppointmentAsync(Appointment);

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
