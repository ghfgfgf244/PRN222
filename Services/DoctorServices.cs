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
    public class DoctorServices : IDoctorServices
    {
        private readonly IDoctorRepositories _iDoctorRepositories;

        public DoctorServices(IDoctorRepositories context)
        {
            _iDoctorRepositories = context;
        }

        public async Task AddSpecialtyToDoctorAsync(int doctorId, int specialtyId)
        {
            await _iDoctorRepositories.AddSpecialtyToDoctorAsync(doctorId, specialtyId);
        }

        public async Task<bool> DeleteLeafAsync(int id)
        {
            return await _iDoctorRepositories.DeleteLeafAsync(id);
        }

        public async Task<List<DoctorLeaf>> GetAllDoctorLeavesAsync()
        {
            return await _iDoctorRepositories.GetAllDoctorLeavesAsync();
        }

        public async Task<List<User>> GetAllDoctorsAsync()
        {
            return await _iDoctorRepositories.GetAllDoctorsAsync();
        }

        public async Task<List<DoctorSpecialty>> GetAllSpecialties()
        {
            return await _iDoctorRepositories.GetAllSpecialties();
        }

        public async Task<User?> GetDoctorByIdAsync(int id)
        {
            return await _iDoctorRepositories.GetDoctorByIdAsync(id);
        }

        public async Task<DoctorLeaf?> GetDoctorLeafByIdAsync(int id)
        {
            return await _iDoctorRepositories.GetDoctorLeafByIdAsync(id);
        }

        public async Task<List<DoctorLeaf>> GetDoctorLeavesByDoctorIdAsync(int doctorId)
        {
            return await _iDoctorRepositories.GetDoctorLeavesByDoctorIdAsync((int)doctorId);
        }

        public async Task<DoctorSpecialty?> GetSpecialtyByIdAsync(int id)
        {
            return await _iDoctorRepositories.GetSpecialtyByIdAsync((int)id);
        }

        public async Task<int?> GetSpecialtyIdByDoctorId(int doctorId)
        {
            return await _iDoctorRepositories.GetSpecialtyIdByDoctorId((int)doctorId);
        }

        public async Task<bool> RegisterDoctorLeaveAsync(DoctorLeaf doctorLeaf)
        {
            return await _iDoctorRepositories.RegisterDoctorLeaveAsync(doctorLeaf);
        }

        public async Task<bool?> ToggleDoctorLeafStatusAsync(int id)
        {
            return await _iDoctorRepositories.ToggleDoctorLeafStatusAsync((int)id);
        }

        public async Task UpdateDoctorSpecialtyAsync(int doctorId, int specialtyId)
        {
            await _iDoctorRepositories.UpdateDoctorSpecialtyAsync(doctorId, specialtyId);
        }
    }
}
