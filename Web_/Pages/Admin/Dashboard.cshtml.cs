using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using System.Linq;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Web_.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IUserServices _userServices;
        private readonly IAppointmentServices _appointmentServices;
        private readonly IDoctorServices _doctorServices;

        public int PatientCount { get; set; }
        public int DoctorCount { get; set; }
        public int AppointmentCount { get; set; }
        public int UsersThisWeek { get; set; }

        public List<(string FullName, string Specialties, int AppointmentCount)> TopDoctors { get; set; }
        public List<string> MethodLabels { get; set; } = new();
        public List<int> MethodData { get; set; } = new();

        public DashboardModel(IUserServices userServices, IAppointmentServices appointmentServices, IDoctorServices doctorServices)
        {
            _userServices = userServices;
            _appointmentServices = appointmentServices;
            _doctorServices = doctorServices;
        }

        public async Task OnGetAsync()
        {
            var users = await _userServices.GetAllUsersAsync();
            var appointments = await _appointmentServices.GetAllAppointmentsAsync();
            var doctors = await _doctorServices.GetAllDoctorsAsync();

            PatientCount = users.Count(u => u.Role == "Patient");
            DoctorCount = doctors.Count(u => u.Role == "Doctor");
            AppointmentCount = appointments.Count;

            UsersThisWeek = users.Count(u => u.CreatedAt.HasValue && u.CreatedAt >= DateTime.Now.AddDays(-7));
            TopDoctors = doctors
                    .Select(doc => new
                    {
                        doc.FullName,
                        Specialties = string.Join(", ", doc.Specialties.Select(s => s.Name)),
                        AppointmentCount = appointments.Count(a => a.DoctorId == doc.UserId)
                    })
                    .OrderByDescending(d => d.AppointmentCount)
                    .Take(5)
                    .Select(d => (d.FullName, d.Specialties, d.AppointmentCount))
                    .ToList();

            var specialtyGroups = appointments
                .Where(a => a.Specialty != null)
                .GroupBy(a => a.Specialty!.Name)
                .Select(g => new { Specialty = g.Key, Count = g.Count() })
                .ToList();

            MethodLabels = specialtyGroups.Select(g => g.Specialty).ToList();
            MethodData = specialtyGroups.Select(g => g.Count).ToList();

        }
    }
}
