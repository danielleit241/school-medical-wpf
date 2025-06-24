using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class HealthCheckResultService(HealthCheckResultRepo healthCheckResultRepo)
    {
        public List<HealthCheckResult> GetAll()
        {
            return healthCheckResultRepo.GetAll();
        }
        public HealthCheckResult? GetById(Guid id)
        {
            return healthCheckResultRepo.GetById(id);
        }
        public void Add(HealthCheckResult healthCheckResult)
        {
            healthCheckResultRepo.Add(healthCheckResult);
        }
        public void Update(HealthCheckResult healthCheckResult)
        {
            healthCheckResultRepo.Update(healthCheckResult);
        }
        public void Delete(Guid id)
        {
            healthCheckResultRepo.Delete(id);
        }
    }
}
