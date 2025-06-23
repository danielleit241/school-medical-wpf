using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class MedicalRegistrationRepo(SchoolmedicalWpfContext _context)
    {
        public List<MedicalRegistration> GetAll()
        {
            return _context.MedicalRegistrations.Include(m => m.Student).Include(m => m.User).ToList();
        }
        public List<MedicalRegistration> GetByStudentId(Guid studentId)
        {
            return _context.MedicalRegistrations.Include(m => m.Student).Include(m => m.User)
                .Where(mr => mr.StudentId == studentId)
                .ToList();
        }
        public MedicalRegistration? GetById(Guid id)
        {
            return _context.MedicalRegistrations
                .Include(m => m.Student)
                .Include(m => m.User)
                .FirstOrDefault(mr => mr.RegistrationId == id);
        }
        public void Add(MedicalRegistration registration)
        {
            _context.MedicalRegistrations.Add(registration);
            _context.SaveChanges();
        }
        public void Update(MedicalRegistration registration)
        {
            _context.MedicalRegistrations.Update(registration);
            _context.SaveChanges();
        }
        public void Delete(Guid id)
        {
            var registration = GetById(id);
            if (registration != null)
            {
                _context.MedicalRegistrations.Remove(registration);
                _context.SaveChanges();
            }
        }
    }
}
