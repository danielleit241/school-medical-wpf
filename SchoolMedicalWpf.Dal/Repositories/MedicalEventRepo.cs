using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class MedicalEventRepo(SchoolmedicalWpfContext _context)
    {
        public List<MedicalEvent> GetAllMedicalEvents()
        {
            return _context.MedicalEvents
                .Include(me => me.Student)
                .ToList();
        }
        public MedicalEvent? GetMedicalEventById(Guid id)
        {
            return _context.MedicalEvents
                .Include(me => me.Student)
                .FirstOrDefault(me => me.EventId == id);
        }
        public void AddMedicalEvent(MedicalEvent medicalEvent)
        {
            _context.MedicalEvents.Add(medicalEvent);
            _context.SaveChanges();
        }
        public void UpdateMedicalEvent(MedicalEvent medicalEvent)
        {
            _context.MedicalEvents.Update(medicalEvent);
            _context.SaveChanges();
        }
        public void DeleteMedicalEvent(Guid id)
        {
            var medicalEvent = _context.MedicalEvents.Find(id);
            if (medicalEvent != null)
            {
                _context.MedicalEvents.Remove(medicalEvent);
                _context.SaveChanges();
            }
        }
    }
}
