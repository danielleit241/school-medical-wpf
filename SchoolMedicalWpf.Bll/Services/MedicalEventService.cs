using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class MedicalEventService(MedicalEventRepo medicalEventRepo)
    {
        public List<MedicalEvent> GetAllMedicalEvents()
        {
            return medicalEventRepo.GetAllMedicalEvents();
        }
        public MedicalEvent? GetMedicalEventById(Guid id)
        {
            return medicalEventRepo.GetMedicalEventById(id);
        }
        public void AddMedicalEvent(MedicalEvent medicalEvent)
        {
            medicalEventRepo.AddMedicalEvent(medicalEvent);
        }
        public void UpdateMedicalEvent(MedicalEvent medicalEvent)
        {
            medicalEventRepo.UpdateMedicalEvent(medicalEvent);
        }
        public void DeleteMedicalEvent(Guid id)
        {
            medicalEventRepo.DeleteMedicalEvent(id);
        }
    }
}
