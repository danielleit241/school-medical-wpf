using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class HealthProfileService
    {
        private readonly HealthProfileRepo _healthProfileRepo;

        public HealthProfileService(HealthProfileRepo healthProfileRepo)
        {
            _healthProfileRepo = healthProfileRepo ?? throw new ArgumentNullException(nameof(healthProfileRepo));
        }

        public List<HealthProfile> GetAllHealthProfiles()
        {
            return _healthProfileRepo.GetAllHealthProfiles();
        }

        public HealthProfile? GetHealthProfileById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid health profile ID.", nameof(id));
            return _healthProfileRepo.GetHealthProfileById(id);
        }

        public HealthProfile? GetHealthProfileByStudentId(Guid? studentId)
        {
            if (studentId == null || studentId == Guid.Empty)
                throw new ArgumentException("Invalid student ID.", nameof(studentId));
            return _healthProfileRepo.GetHealthProfileByStudentId(studentId);
        }

        public void Add(HealthProfile healthProfile)
        {
            if (healthProfile == null)
                throw new ArgumentNullException(nameof(healthProfile), "Health profile cannot be null");
            _healthProfileRepo.Add(healthProfile);
        }
    }
}
