using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class HealthProfileRepo
    {
        private readonly SchoolmedicalWpfContext _context;

        public HealthProfileRepo(SchoolmedicalWpfContext context)
        {
            _context = context;
        }

        public HealthProfile? GetHealthProfileByStudentId(Guid? studentId)
        {
            return _context.HealthProfiles.Include(hp => hp.Student).FirstOrDefault(hp => hp.StudentId == studentId);
        }

        public void Add(HealthProfile healthProfile)
        {
            if (healthProfile == null)
            {
                throw new ArgumentNullException(nameof(healthProfile), "Health profile cannot be null");
            }
            _context.HealthProfiles.Add(healthProfile);
            _context.SaveChanges();
        }
    }
}
