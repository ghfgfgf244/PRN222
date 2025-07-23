using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IDoctorServices
    {
        Task<List<DoctorLeaf>> GetAllDoctorLeavesAsync();
        Task<List<DoctorLeaf>> GetDoctorLeavesByDoctorIdAsync(int doctorId);
        Task<bool?> ToggleDoctorLeafStatusAsync(int id);
        Task<bool> DeleteLeafAsync(int id);
        Task<DoctorLeaf?> GetDoctorLeafByIdAsync(int id);
        Task<bool> RegisterDoctorLeaveAsync(DoctorLeaf doctorLeaf);
        Task<List<DoctorSpecialty>> GetAllSpecialties();

        Task<DoctorSpecialty?> GetSpecialtyByIdAsync(int id);
        Task<List<User>> GetAllDoctorsAsync();
        Task AddSpecialtyToDoctorAsync(int doctorId, int specialtyId);
        Task UpdateDoctorSpecialtyAsync(int doctorId, int specialtyId);
        Task<int?> GetSpecialtyIdByDoctorId(int doctorId);
        Task<User?> GetDoctorByIdAsync(int id);
    }
}
