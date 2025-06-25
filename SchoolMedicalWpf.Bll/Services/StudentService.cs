using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Bll.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository;

        public StudentService(StudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<List<Student>> GetAllStudents()
        {
            return await _studentRepository.GetAllStudents();
        }

        public async Task<Student?> GetStudentById(Guid studentId)
        {
            return await _studentRepository.GetStudentById(studentId);
        }

        public async Task AddStudent(Student student)
        {
            student.StudentId = Guid.NewGuid();
            await _studentRepository.AddStudent(student);
        }

        public async Task UpdateStudent(Student student)
        {
            await _studentRepository.UpdateStudent(student);
        }

        public async Task DeleteStudent(Guid studentId)
        {
            await _studentRepository.DeleteStudent(studentId);
        }

        public async Task<List<Student>> GetStudentsByUserId(Guid userId)
        {
            return await _studentRepository.GetStudentsByUserId(userId);
        }
    }
}