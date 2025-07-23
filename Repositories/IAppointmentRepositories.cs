using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IAppointmentRepositories
    {
        Task<List<ExamMethod>> GetMethodsBySpecialtyId(int specialtyId);
        Task<List<User>> GetAvailableDoctors(int specialtyId, int slotId, DateOnly date);
        Task<List<TimeSlot>> GetAllSlots();
        Task<bool> CreateAppointmentAsync(Appointment appointment);
        Task<List<Appointment>> GetAppointmentsByRegisteredByAsync(int userId);
        Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task<bool> CancelAppointmentAsync(int appointmentId);
        Task<bool> UpdateAppointmentAsync(Appointment updatedAppointment);
        Task<Appointment?> GetAppointmentByIdAsync(int appointmentId);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string newStatus);
        Task<(List<Appointment> Appointments, int TotalPages)> GetPagedAppointmentsAsync(IQueryable<Appointment> query, string? searchTerm, int pageNumber, int pageSize);
        IQueryable<Appointment> GetAppointmentsByDoctorQuery(int doctorId);
        IQueryable<Appointment> GetAppointmentsByPatientQuery(int registeredById);
        Task<List<Appointment>> GetAllAppointmentsAsync();
    }
}
