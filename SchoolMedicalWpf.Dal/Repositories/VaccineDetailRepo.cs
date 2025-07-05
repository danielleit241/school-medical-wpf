using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class VaccineDetailRepo
    {
        private readonly SchoolmedicalWpfContext _context;
        public VaccineDetailRepo(SchoolmedicalWpfContext context)
        {
            _context = context;
        }

        public async Task<List<VaccineDetail>> GetAllVaccineDetailsAsync()
        {
            return await _context.VaccineDetails.ToListAsync();
        }
        public async Task<VaccineDetail?> GetVaccineDetailByIdAsync(Guid vaccineId)
        {
            return await _context.VaccineDetails.FindAsync(vaccineId);
        }
    }
}
