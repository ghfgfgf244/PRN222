using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<List<ExamMethod>> GetExamMethodsBySpecialtyAsync(int specialtyId)
        {
            return await _context.ExamMethods
                .Where(m => m.SpecialtyId == specialtyId)
                .ToListAsync();
        }
        // 1. Tạo lịch hẹn mới
        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        // 2. Kiểm tra slot có bị trùng không
        public async Task<bool> IsSlotAvailableAsync(int specialtyId, DateOnly date, int slotId)
        {
            return !await _context.Appointments.AnyAsync(a =>
                a.SpecialtyId == specialtyId &&
                a.AppointmentDate == date &&
                a.SlotId == slotId);
        }

        // 3. Lấy danh sách lịch của 1 user (bệnh nhân)
        public async Task<List<Appointment>> GetAppointmentsByUserAsync(int userId)
        {
            return await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.Patient.RegisteredBy == userId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        // 4. Hủy lịch hẹn (chỉ khi còn Pending)
        public async Task<bool> CancelAppointmentAsync(int appointmentId)
        {
            var appt = await _context.Appointments.FindAsync(appointmentId);
            if (appt == null || appt.Status != "Pending") return false;

            appt.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Xem chi tiết 1 lịch hẹn
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

        // 6. Lấy danh sách slot đã đặt trong ngày theo chuyên ngành
        public async Task<List<int?>> GetTakenSlotIdsAsync(int specialtyId, DateOnly date)
        {
            return await _context.Appointments
                .Where(a => a.SpecialtyId == specialtyId && a.AppointmentDate == date)
                .Select(a => a.SlotId)
                .ToListAsync();
        }
    }
}
