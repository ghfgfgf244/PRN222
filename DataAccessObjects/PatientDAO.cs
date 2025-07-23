using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        public async Task<bool> AddPatientAsync(Patient patient)
        {
            if (patient == null) return false;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();
            return true;
        }

        // 3. Cập nhật thông tin bệnh nhân
        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            var existingPatient = await _context.Patients.FindAsync(patient.PatientId);
            if (existingPatient == null) return false;

            existingPatient.FullName = patient.FullName;
            existingPatient.Age = patient.Age;
            existingPatient.Gender = patient.Gender;
            existingPatient.Note = patient.Note;
            
            await _context.SaveChangesAsync();
            Console.WriteLine("patient");

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
            return await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == patientId);
        }

    }
}
