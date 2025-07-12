using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessObjects
{
    public class PatientDAO
    {
        private readonly AppointmentsDbContext _context;

        public PatientDAO(AppointmentsDbContext context)
        {
            _context = context;
        }

        // 1. Lấy danh sách bệnh nhân do 1 user đăng ký
        public async Task<List<Patient>> GetPatientsByUserAsync(int userId)
        {
            return await _context.Patients
                .Where(p => p.RegisteredBy == userId)
                .ToListAsync();
        }

        // 2. Thêm bệnh nhân mới (người thân)
        public async Task<Patient> AddPatientAsync(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        // 3. Cập nhật thông tin bệnh nhân
        public async Task<bool> UpdatePatientAsync(Patient updatedPatient)
        {
            var patient = await _context.Patients.FindAsync(updatedPatient.PatientId);
            if (patient == null || patient.RegisteredBy != updatedPatient.RegisteredBy) return false;

            patient.FullName = updatedPatient.FullName;
            patient.Age = updatedPatient.Age;
            patient.Gender = updatedPatient.Gender;
            patient.Note = updatedPatient.Note;

            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Xóa bệnh nhân (chỉ khi chưa có lịch khám)
        public async Task<bool> DeletePatientAsync(int patientId)
        {
            var patient = await _context.Patients
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);

            if (patient == null || patient.Appointments.Any()) return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Lấy thông tin 1 bệnh nhân cụ thể
        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.Patients
                .Include(p => p.RegisteredByNavigation)
                .FirstOrDefaultAsync(p => p.PatientId == patientId);
        }
    }
}
