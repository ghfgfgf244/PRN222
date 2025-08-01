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
    public class PatientServices : IPatientServices
    {
        private readonly IPatientRepositories _context;

        public PatientServices(IPatientRepositories context)
        {
            _context = context;
        }
        public async Task<bool> AddPatientAsync(Patient patient)
        {
            return await _context.AddPatientAsync(patient);
        }
        public async Task<Patient?> GetPatientByIdAsync(int patientId)
        {
            return await _context.GetPatientByIdAsync(patientId);
        }

        public async Task<bool> UpdatePatientAsync(Patient patient)
        {
            return await (_context.UpdatePatientAsync(patient));
        }
    }
}
