using BusinessObjects;
using DataAccessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class PatientRepositories : IPatientRepositories
    {
        private readonly PatientDAO _context;

        public PatientRepositories(PatientDAO context)
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
