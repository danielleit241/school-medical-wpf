using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class VaccinationScheduleRepo
    {
        private readonly SchoolmedicalWpfContext _context;
        public VaccinationScheduleRepo(SchoolmedicalWpfContext context)
        {
            _context = context;
        }
        public List<VaccinationSchedule> GetAllVaccinationSchedules()
        {
            return _context.VaccinationSchedules
                .Include(v => v.Vaccine)
                .ToList();
        }
        public VaccinationSchedule? GetVaccinationScheduleById(Guid id)
        {
            return _context.VaccinationSchedules
                .Include(v => v.Vaccine)
                .FirstOrDefault(v => v.ScheduleId == id);
        }
        public VaccinationSchedule Add(VaccinationSchedule schedule)
        {
            _context.VaccinationSchedules.Add(schedule);
            _context.SaveChanges();
            return schedule;
        }
        public VaccinationSchedule Update(VaccinationSchedule schedule)
        {
            _context.VaccinationSchedules.Update(schedule);
            _context.SaveChanges();
            return schedule;
        }
        public void Delete(Guid id)
        {
            var schedule = _context.VaccinationSchedules.Find(id);
            if (schedule != null)
            {
                _context.VaccinationSchedules.Remove(schedule);
                _context.SaveChanges();
            }
        }
    }
}
