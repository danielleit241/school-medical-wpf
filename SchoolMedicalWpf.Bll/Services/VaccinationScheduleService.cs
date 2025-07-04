using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class VaccinationScheduleService
    {
        private readonly VaccinationScheduleRepo _vaccinationScheduleRepo;
        public VaccinationScheduleService(VaccinationScheduleRepo vaccinationScheduleRepo)
        {
            _vaccinationScheduleRepo = vaccinationScheduleRepo;
        }
        public List<VaccinationSchedule> GetAllVaccinationSchedules()
        {
            return _vaccinationScheduleRepo.GetAllVaccinationSchedules();
        }
        public VaccinationSchedule? GetVaccinationScheduleById(Guid id)
        {
            return _vaccinationScheduleRepo.GetVaccinationScheduleById(id);
        }
        public void AddVaccinationSchedule(VaccinationSchedule vaccinationSchedule)
        {
            _vaccinationScheduleRepo.Add(vaccinationSchedule);
        }
        public void UpdateVaccinationSchedule(VaccinationSchedule vaccinationSchedule)
        {
            _vaccinationScheduleRepo.Update(vaccinationSchedule);
        }
        public void DeleteVaccinationSchedule(Guid id)
        {
            _vaccinationScheduleRepo.Delete(id);
        }
    }
}
