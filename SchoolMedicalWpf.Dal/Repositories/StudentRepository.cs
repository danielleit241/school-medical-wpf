using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class StudentRepository
    {
        private SchoolmedicalWpfContext _context;
        public StudentRepository(SchoolmedicalWpfContext context)
        {
            _context = context;
        }

        public List<Student> GetAll()
        {
            return _context.Students.Include(s => s.HealthProfiles).ToList();
        }

        public List<Student> GetStudentsByUserId(Guid userId)
        {
            return _context.Students.Include(s => s.HealthProfiles).Where(s => s.UserId == userId)
                .ToList();
        }
        public Student? GetStudentById(Guid studentId)
        {
            return _context.Students.Include(s => s.HealthProfiles)
                .FirstOrDefault(s => s.StudentId == studentId);
        }
    }
}
