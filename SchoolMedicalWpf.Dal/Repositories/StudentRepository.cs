using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class StudentRepository
    {
        private SchoolmedicalWpfContext _context;
        public List<Student> GetStudentsByUserId(Guid userId)
        {
            _context = new();
            return _context.Students.Where(s => s.UserId == userId)
                .ToList();
        }
    }
}
