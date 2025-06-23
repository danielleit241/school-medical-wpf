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
            return _context.Students.ToList();
        }

        public List<Student> GetStudentsByUserId(Guid userId)
        {
            return _context.Students.Where(s => s.UserId == userId)
                .ToList();
        }
    }
}
