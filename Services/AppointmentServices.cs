using BusinessObjects;
using DataAccessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AppointmentServices : IAppointmentServices
    {
        private readonly IAppointmentRepositories _context;

        public AppointmentServices(IAppointmentRepositories context)
        {
            _context = context;
        }
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            return await _context.CancelAppointmentAsync(appointmentId);
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            return await _context.CreateAppointmentAsync(appointment);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.GetAllAppointmentsAsync();
        }

        public async Task<List<TimeSlot>> GetAllSlots()
        {
            return await _context.GetAllSlots();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _context.GetAppointmentsByDoctorIdAsync((int)doctorId);
        }

        public IQueryable<Appointment> GetAppointmentsByDoctorQuery(int doctorId)
        {
            return _context.GetAppointmentsByDoctorQuery((int)doctorId);
        }

        public IQueryable<Appointment> GetAppointmentsByPatientQuery(int registeredById)
        {
            return _context.GetAppointmentsByPatientQuery((int)registeredById);
        }

        public async Task<List<Appointment>> GetAppointmentsByRegisteredByAsync(int userId)
        {
            return await _context.GetAppointmentsByRegisteredByAsync((int)userId);
        }

        public async Task<List<User>> GetAvailableDoctors(int specialtyId, int slotId, DateOnly date)
        {
            return await _context.GetAvailableDoctors(specialtyId, slotId, date);
        }

        public async Task<List<ExamMethod>> GetMethodsBySpecialtyId(int specialtyId)
        {
            return await _context.GetMethodsBySpecialtyId((int)specialtyId);
        }

        public async Task<(List<Appointment> Appointments, int TotalPages)> GetPagedAppointmentsAsync(IQueryable<Appointment> query, string? searchTerm, int pageNumber, int pageSize)
        {
            return await _context.GetPagedAppointmentsAsync(query, searchTerm, pageNumber, pageSize);
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment updatedAppointment)
        {
            return await _context.UpdateAppointmentAsync(updatedAppointment);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string newStatus)
        {
            return await _context.UpdateAppointmentStatusAsync(appointmentId, newStatus);
        }
    }
}
