using SchoolMedicalWpf.Dal.Entities;
using SchoolMedicalWpf.Dal.Repositories;

namespace SchoolMedicalWpf.Bll.Services
{
    public class StudentService
    {
        private readonly StudentRepository _studentRepository = new();

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
    }
}
