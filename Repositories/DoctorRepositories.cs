using BusinessObjects;
using DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class DoctorRepositories : IDoctorRepositories
    {
        private readonly DoctorDAO _doctorDAO;

        public DoctorRepositories(DoctorDAO context)
        {
            _doctorDAO = context;
        }

        public async Task AddSpecialtyToDoctorAsync(int doctorId, int specialtyId)
        {
            await _doctorDAO.AddSpecialtyToDoctorAsync(doctorId, specialtyId);
        }

        public async Task<bool> DeleteLeafAsync(int id)
        {
            return await _doctorDAO.DeleteLeafAsync(id);
        }

        public async Task<List<DoctorLeaf>> GetAllDoctorLeavesAsync()
        {
            return await _doctorDAO.GetAllDoctorLeavesAsync();
        }

        public async Task<List<User>> GetAllDoctorsAsync()
        {
            return await _doctorDAO.GetAllDoctorsAsync();
        }

        public async Task<List<DoctorSpecialty>> GetAllSpecialties()
        {
            return await _doctorDAO.GetAllSpecialties();
        }

        public async Task<User?> GetDoctorByIdAsync(int id)
        {
            return await _doctorDAO.GetDoctorByIdAsync(id);
        }

        public async Task<DoctorLeaf?> GetDoctorLeafByIdAsync(int id)
        {
            return await _doctorDAO.GetDoctorLeafByIdAsync(id);
        }

        public async Task<List<DoctorLeaf>> GetDoctorLeavesByDoctorIdAsync(int doctorId)
        {
            return await _doctorDAO.GetDoctorLeavesByDoctorIdAsync((int)doctorId);
        }

        public async Task<DoctorSpecialty?> GetSpecialtyByIdAsync(int id)
        {
            return await _doctorDAO.GetSpecialtyByIdAsync((int)id);
        }

        public async Task<int?> GetSpecialtyIdByDoctorId(int doctorId)
        {
            return await _doctorDAO.GetSpecialtyIdByDoctorId((int)doctorId);
        }

        public async Task<bool> RegisterDoctorLeaveAsync(DoctorLeaf doctorLeaf)
        {
            return await _doctorDAO.RegisterDoctorLeaveAsync(doctorLeaf);
        }

        public async Task<bool?> ToggleDoctorLeafStatusAsync(int id)
        {
            return await _doctorDAO.ToggleDoctorLeafStatusAsync((int)id);
        }

        public async Task UpdateDoctorSpecialtyAsync(int doctorId, int specialtyId)
        {
            await _doctorDAO.UpdateDoctorSpecialtyAsync(doctorId, specialtyId);
        }
    }
}
