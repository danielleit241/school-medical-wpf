using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class VaccinationResultService(VaccinationResultRepo vaccinationResultRepo)
    {
        public List<VaccinationResult> GetAllVaccinationResults()
        {
            return vaccinationResultRepo.GetAllVaccinationResults();
        }
        public VaccinationResult? GetVaccinationResultById(Guid id)
        {
            return vaccinationResultRepo.GetVaccinationResultById(id);
        }
        public void AddVaccinationResult(VaccinationResult vaccinationResult)
        {
            vaccinationResultRepo.AddVaccinationResult(vaccinationResult);
        }
        public void UpdateVaccinationResult(VaccinationResult vaccinationResult)
        {
            vaccinationResultRepo.UpdateVaccinationResult(vaccinationResult);
        }
        public void DeleteVaccinationResult(Guid id)
        {
            vaccinationResultRepo.DeleteVaccinationResult(id);
        }
    }
}
