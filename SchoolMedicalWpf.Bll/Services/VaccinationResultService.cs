using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class VaccinationResultService(VaccinationResultRepo vaccinationResultRepo)
    {
        public List<VaccinationResult> GetAllVaccinationResults()
        {
            return vaccinationResultRepo.GetAll();
        }
        public VaccinationResult? GetVaccinationResultById(Guid id)
        {
            return vaccinationResultRepo.GetById(id);
        }
        public void AddVaccinationResult(VaccinationResult vaccinationResult)
        {
            vaccinationResultRepo.Add(vaccinationResult);
        }
        public void UpdateVaccinationResult(VaccinationResult vaccinationResult)
        {
            vaccinationResultRepo.Update(vaccinationResult);
        }
        public void DeleteVaccinationResult(Guid id)
        {
            vaccinationResultRepo.Delete(id);
        }
    }
}
