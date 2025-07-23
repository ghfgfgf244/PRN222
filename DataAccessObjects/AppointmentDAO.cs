using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DataAccessObjects
{
    public class AppointmentDAO
    {
        private readonly AppointmentsDbContext _context;

        public AppointmentDAO(AppointmentsDbContext context)
        {
            _context = context;
        }
        // 8. Lấy danh sách phương pháp khám theo chuyên ngành
        public async Task<List<ExamMethod>> GetMethodsBySpecialtyId(int specialtyId)
        {
            return await _context.ExamMethods
                .Where(m => m.SpecialtyId == specialtyId)
                .ToListAsync();
        }

        // 1. Tạo lịch hẹn mới
        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            if (appointment == null) return false;

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        // 2. Kiểm tra slot có bị trùng không
        public async Task<bool> IsSlotAvailableAsync(int specialtyId, DateOnly date, int slotId)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.SpecialtyId == specialtyId &&
                a.AppointmentDate == date &&
                a.SlotId == slotId);
        }

        //  Lấy danh sách lịch của 1 user (bệnh nhân)
        public async Task<List<Appointment>> GetAppointmentsByRegisteredByAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.Patient.RegisteredBy == userId && a.Status != "Cancelled")
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }
        //  Lấy danh sách lịch của 1 user (bác sĩ)
        public async Task<List<Appointment>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.DoctorId == doctorId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }


        //  Hủy lịch hẹn (chỉ khi còn Pending)
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null || appt.Status != "Pending") return false;

            appt.Status = "Cancelled";
            Console.WriteLine(appt.Status);
            await _context.SaveChangesAsync();
            return true;
        }

        //  update lịch hẹn (chỉ khi còn Pending)
        public async Task<bool> UpdateAppointmentAsync(Appointment updatedAppointment)
        {
            var appointment = await _context.Appointments.FindAsync(updatedAppointment.AppointmentId);
            if (appointment == null) return false;

            if (!string.Equals(appointment.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                return false;

            appointment.AppointmentDate = updatedAppointment.AppointmentDate;
            appointment.SlotId = updatedAppointment.SlotId;
            appointment.SpecialtyId = updatedAppointment.SpecialtyId;
            appointment.MethodId = updatedAppointment.MethodId;
            appointment.DoctorId = updatedAppointment.DoctorId;

            await _context.SaveChangesAsync();
            Console.WriteLine("appointment");

            return true;
        }


        //  Xem chi tiết 1 lịch hẹn
        public async Task<Appointment?> GetAppointmentByIdAsync(int appointmentId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        //  Lấy danh sách bác sĩ có sẵn
        public async Task<List<User>> GetAvailableDoctors(int specialtyId, int slotId, DateOnly date)
        {
            var doctorsInSpecialty = await _context.Users
                .Include(u => u.Specialties)
                .Where(u => u.Role == "Doctor" && u.Specialties.Any(s => s.SpecialtyId == specialtyId))
                .ToListAsync();

            var bookedDoctorIds = await _context.Appointments
                .Where(a => a.AppointmentDate == date && a.SlotId == slotId)
                .Select(a => a.DoctorId)
                .ToListAsync();

            var availableDoctors = doctorsInSpecialty
                .Where(d => !bookedDoctorIds.Contains(d.UserId))
                .ToList();

            return availableDoctors;
        }

        // Lấy danh sách slot
        public async Task<List<TimeSlot>> GetAllSlots()
        {
            return await _context.TimeSlots.ToListAsync();
        }

        public async Task<List<Appointment>> GetAllAppointmentsAsync()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Method)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
            return appointments;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string newStatus)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null)
                return false;

            appointment.Status = newStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(List<Appointment> Appointments, int TotalPages)> GetPagedAppointmentsAsync(IQueryable<Appointment> query, string? searchTerm,int pageNumber,int pageSize)
        {
            // Tìm kiếm theo searchTerm nếu có
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(a =>
                    (a.Patient != null && a.Patient.FullName.Contains(searchTerm)) ||
                    (a.Specialty != null && a.Specialty.Name.Contains(searchTerm)) ||
                    (a.Method != null && a.Method.Name.Contains(searchTerm))
                );
            }

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var appointments = await query
                .OrderByDescending(a => a.AppointmentDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (appointments, totalPages);
        }
        public IQueryable<Appointment> GetAppointmentsByDoctorQuery(int doctorId)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.DoctorId == doctorId);
        }

        public IQueryable<Appointment> GetAppointmentsByPatientQuery(int registeredById)
        {
            return _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.Patient.RegisteredBy == registeredById && a.Status != "Cancelled");
        }

    }
}
