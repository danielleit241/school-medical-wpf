using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class MedicalRegistrationService(MedicalRegistrationRepo medicalRegistrationRepo)
    {
        public List<MedicalRegistration> GetAllRegistrations()
        {
            return medicalRegistrationRepo.GetAll();
        }
        public List<MedicalRegistration> GetRegistrationsByStudentId(Guid studentId)
        {
            return medicalRegistrationRepo.GetByStudentId(studentId);
        }
        public MedicalRegistration? GetRegistrationById(Guid id)
        {
            return medicalRegistrationRepo.GetById(id);
        }
        public void AddRegistration(MedicalRegistration registration)
        {
            medicalRegistrationRepo.Add(registration);
        }
        public void UpdateRegistration(MedicalRegistration registration)
        {
            medicalRegistrationRepo.Update(registration);
        }
        public void DeleteRegistration(Guid id)
        {
            medicalRegistrationRepo.Delete(id);
        }
    }
}
