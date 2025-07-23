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
        public async Task<List<int?>> GetTakenSlotIdsAsync(int doctorId, DateOnly date)
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
        public async Task<bool> RegisterDoctorLeaveAsync(DoctorLeaf doctorLeaf)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var minAllowedDate = today.AddDays(7);

            if (doctorLeaf.LeaveDate < minAllowedDate)
            {
                return false;
            }

            if (await IsDoctorOnLeaveAsync(doctorLeaf.DoctorId.Value, doctorLeaf.LeaveDate)) return false;

            _context.DoctorLeaves.Add(doctorLeaf);
            await _context.SaveChangesAsync();
            return true;
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


        //Doctor Leaf
        public async Task<List<DoctorLeaf>> GetAllDoctorLeavesAsync()
        {
            return await _context.DoctorLeaves
                .Include(l => l.Doctor) // Lấy thêm thông tin bác sĩ (User)
                .OrderByDescending(l => l.LeaveDate)
                .ToListAsync();
        }
        public async Task<List<DoctorLeaf>> GetDoctorLeavesByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorLeaves
                .Include(l => l.Doctor) // Nếu bạn cần thông tin bác sĩ trong kết quả
                .Where(l => l.DoctorId == doctorId)
                .OrderByDescending(l => l.LeaveDate)
                .ToListAsync();
        }
        public async Task<bool?> ToggleDoctorLeafStatusAsync(int id)
        {
            var leaf = await _context.DoctorLeaves.FindAsync(id);
            if (leaf == null) return null;

            leaf.IsActive = !leaf.IsActive;
            await _context.SaveChangesAsync();
            return leaf.IsActive;
        }
        public async Task<bool> DeleteLeafAsync(int id)
        {
            var leaf = await _context.DoctorLeaves.FindAsync(id);
            if (leaf == null) return false;

            _context.DoctorLeaves.Remove(leaf);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<DoctorLeaf?> GetDoctorLeafByIdAsync(int id)
        {
            return await _context.DoctorLeaves.FirstOrDefaultAsync(u => u.LeaveId == id);
        }

        public async Task<User?> GetDoctorByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserId == id && u.Role == "Doctor");
        }

        //Specialty
        public async Task<List<DoctorSpecialty>> GetAllSpecialties()
        {
            return await _context.DoctorSpecialties.ToListAsync();
        }

        public async Task<DoctorSpecialty?> GetSpecialtyByIdAsync(int id)
        {
            return await _context.DoctorSpecialties.FindAsync(id);
        }

        public async Task AddSpecialtyToDoctorAsync(int doctorId, int specialtyId)
        {
            var doctor = await _context.Users
                .Include(u => u.Specialties)
                .FirstOrDefaultAsync(u => u.UserId == doctorId && u.Role == "Doctor");

            if (doctor == null)
                throw new Exception("Không tìm thấy bác sĩ.");

            var specialty = await _context.DoctorSpecialties.FindAsync(specialtyId);
            if (specialty == null)
                throw new Exception("Không tìm thấy chuyên ngành.");

            // Tránh thêm trùng
            if (!doctor.Specialties.Any(s => s.SpecialtyId == specialtyId))
            {
                doctor.Specialties.Add(specialty);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateDoctorSpecialtyAsync(int doctorId, int newSpecialtyId)
        {
            var doctor = await _context.Users
                .Include(u => u.Specialties)
                .FirstOrDefaultAsync(u => u.UserId == doctorId && u.Role == "Doctor");

            if (doctor == null)
                throw new Exception("Không tìm thấy bác sĩ.");

            // Kiểm tra xem chuyên ngành mới có tồn tại không
            var newSpecialty = await _context.DoctorSpecialties.FindAsync(newSpecialtyId);
            if (newSpecialty == null)
                throw new Exception("Không tìm thấy chuyên ngành mới.");

            // Xóa hết chuyên ngành cũ (nếu có)
            doctor.Specialties.Clear();

            // Thêm chuyên ngành mới
            doctor.Specialties.Add(newSpecialty);

            await _context.SaveChangesAsync();
        }
        public async Task<int?> GetSpecialtyIdByDoctorId(int doctorId)
        {
            var doctor = await _context.Users
                .Include(u => u.Specialties)
                .FirstOrDefaultAsync(u => u.UserId == doctorId && u.Role == "Doctor");

            if (doctor == null)
                return null;

            // Giả sử mỗi bác sĩ chỉ có 1 chuyên ngành
            return doctor.Specialties.FirstOrDefault()?.SpecialtyId;
        }
        public async Task<List<User>> GetAllDoctorsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Doctor")
                .Include(d => d.Specialties)
                .ToListAsync();
        }
    }
}
