using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;
        public StudentService(StudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public List<Student> GetStudents()
        {
            var res = _studentRepository.GetAll();
            if (res == null)
            {
                return [];
            }
            return res;
        }

        public List<Student> GetStudentsByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return [];
            }
            var res = _studentRepository.GetStudentsByUserId(userId);
            if (res == null)
            {
                return [];
            }
            return res;
        }

        public Student? GetStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
            {
                return null;
            }
            var res = _studentRepository.GetStudentById(studentId);
            if (res == null)
            {
                return null;
            }
            return res;
        }
    }
}
