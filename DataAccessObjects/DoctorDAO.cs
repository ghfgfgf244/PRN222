using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class DoctorDAO
    {
        private readonly AppointmentsDbContext _context;

        public DoctorDAO(AppointmentsDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách lịch khám theo bác sĩ và ngày
        public async Task<List<Appointment>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateOnly date)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Slot)
                .Include(a => a.Specialty)
                .Include(a => a.Method)
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .ToListAsync();
        }

        // 2. Lấy danh sách khung giờ đã bị đặt bởi bác sĩ trong ngày
        public async Task<List<int>> GetTakenSlotIdsAsync(int doctorId, DateOnly date)
        {
            return await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
                .Select(a => a.SlotId)
                .ToListAsync();
        }

        // 3. Kiểm tra bác sĩ có nghỉ phép ngày đó không
        public async Task<bool> IsDoctorOnLeaveAsync(int doctorId, DateOnly date)
        {
            return await _context.DoctorLeaves
                .AnyAsync(l => l.DoctorId == doctorId && l.LeaveDate == date);
        }

        // 4. Đăng ký nghỉ phép (nếu chưa có ngày đó)
        public async Task<bool> RegisterDoctorLeaveAsync(int doctorId, DateOnly leaveDate, string? reason)
        {
            if (await IsDoctorOnLeaveAsync(doctorId, leaveDate)) return false;

            var leave = new DoctorLeaf
            {
                DoctorId = doctorId,
                LeaveDate = leaveDate,
                Reason = reason,
                CreatedAt = DateTime.Now
            };

            _context.DoctorLeaves.Add(leave);
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Lấy danh sách ngày nghỉ của bác sĩ
        public async Task<List<DoctorLeaf>> GetDoctorLeavesAsync(int doctorId)
        {
            return await _context.DoctorLeaves
                .Where(l => l.DoctorId == doctorId)
                .OrderByDescending(l => l.LeaveDate)
                .ToListAsync();
        }

        // 6. Lấy danh sách bác sĩ theo chuyên ngành dựa vào bảng Appointment
        public async Task<List<User>> GetDoctorsBySpecialtyAsync(int specialtyId)
        {
            return await _context.Users
                .Where(u => u.Role == "Doctor" && u.IsActive &&
                            _context.Appointments.Any(a => a.DoctorId == u.UserId && a.SpecialtyId == specialtyId))
                .ToListAsync();
        }

        // 7. Lấy thông tin bác sĩ theo ID (bao gồm danh sách lịch nghỉ)
        public async Task<User?> GetDoctorWithLeavesAsync(int doctorId)
        {
            return await _context.Users
                .Include(u => u.DoctorLeaves)
                .FirstOrDefaultAsync(u => u.UserId == doctorId && u.Role == "Doctor");
        }

    }
}
