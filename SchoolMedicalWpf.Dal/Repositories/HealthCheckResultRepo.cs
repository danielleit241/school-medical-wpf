using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class HealthCheckResultRepo(SchoolmedicalWpfContext _context)
    {
        public List<HealthCheckResult> GetAll()
        {
            return _context.HealthCheckResults.Include(hcr => hcr.HealthProfile).ThenInclude(hp => hp.Student).ToList();
        }
        public HealthCheckResult? GetById(Guid id)
        {
            return _context.HealthCheckResults.Include(hcr => hcr.HealthProfile).ThenInclude(hp => hp.Student).FirstOrDefault(h => h.ResultId == id);
        }
        public void Add(HealthCheckResult healthCheckResult)
        {
            _context.HealthCheckResults.Add(healthCheckResult);
            _context.SaveChanges();
        }
        public void Update(HealthCheckResult healthCheckResult)
        {
            _context.HealthCheckResults.Update(healthCheckResult);
            _context.SaveChanges();
        }
        public void Delete(Guid id)
        {
            var healthCheckResult = GetById(id);
            if (healthCheckResult != null)
            {
                _context.HealthCheckResults.Remove(healthCheckResult);
                _context.SaveChanges();
            }
        }
    }
}
