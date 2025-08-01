using BusinessObjects;
using DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class AppointmentRepositories : IAppointmentRepositories
    {
        private readonly AppointmentDAO _appointmentDAO;

        public AppointmentRepositories(AppointmentDAO context)
        {
            _appointmentDAO = context;
        }

        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            return await _appointmentDAO.CancelAppointmentAsync(appointmentId);
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            return await _appointmentDAO.CreateAppointmentAsync(appointment);
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            return await _appointmentDAO.GetAllAppointmentsAsync();
        }

        public async Task<List<TimeSlot>> GetAllSlots()
        {
            return await _appointmentDAO.GetAllSlots();
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _appointmentDAO.GetAppointmentByIdAsync(appointmentId);
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _appointmentDAO.GetAppointmentsByDoctorIdAsync((int)doctorId);
        }

        public IQueryable<Appointment> GetAppointmentsByDoctorQuery(int doctorId)
        {
            return _appointmentDAO.GetAppointmentsByDoctorQuery((int)doctorId);
        }

        public IQueryable<Appointment> GetAppointmentsByPatientQuery(int registeredById)
        {
            return _appointmentDAO.GetAppointmentsByPatientQuery((int)registeredById);
        }

        public async Task<List<Appointment>> GetAppointmentsByRegisteredByAsync(int userId)
        {
            return await _appointmentDAO.GetAppointmentsByRegisteredByAsync((int)userId);
        }

        public async Task<List<User>> GetAvailableDoctors(int specialtyId, int slotId, DateOnly date)
        {
            return await _appointmentDAO.GetAvailableDoctors(specialtyId, slotId, date);
        }

        public async Task<List<ExamMethod>> GetMethodsBySpecialtyId(int specialtyId)
        {
            return await _appointmentDAO.GetMethodsBySpecialtyId((int)specialtyId);
        }

        public async Task<(List<Appointment> Appointments, int TotalPages)> GetPagedAppointmentsAsync(IQueryable<Appointment> query, string? searchTerm, int pageNumber, int pageSize)
        {
            return await _appointmentDAO.GetPagedAppointmentsAsync(query,searchTerm, pageNumber, pageSize);   
        }

        public async Task<bool> UpdateAppointmentAsync(Appointment updatedAppointment)
        {
            return await _appointmentDAO.UpdateAppointmentAsync(updatedAppointment);
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string newStatus)
        {
            return await _appointmentDAO.UpdateAppointmentStatusAsync(appointmentId, newStatus);
        }
    }
}
