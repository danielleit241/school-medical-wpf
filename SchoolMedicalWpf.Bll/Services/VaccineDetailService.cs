using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class VaccineDetailService
    {
        private readonly VaccineDetailRepo _vaccineDetailRepo;
        public VaccineDetailService(VaccineDetailRepo vaccineDetailRepo)
        {
            _vaccineDetailRepo = vaccineDetailRepo;
        }
        public async Task<List<VaccineDetail>> GetAllVaccineDetailsAsync()
        {
            return await _vaccineDetailRepo.GetAllVaccineDetailsAsync();
        }
        public async Task<VaccineDetail?> GetVaccineDetailByIdAsync(Guid vaccineId)
        {
            return await _vaccineDetailRepo.GetVaccineDetailByIdAsync(vaccineId);
        }
    }
}
