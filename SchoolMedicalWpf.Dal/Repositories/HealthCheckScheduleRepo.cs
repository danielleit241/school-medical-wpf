namespace SchoolMedicalWpf.Dal.Repositories
{
    using Entities;

    public class HealthCheckScheduleRepo
    {
        private readonly SchoolmedicalWpfContext _context;

        public HealthCheckScheduleRepo(SchoolmedicalWpfContext context)
        {
            _context = context;
        }

        public List<HealthCheckSchedule> GetAllHealthCheckSchedules()
        {
            return _context.HealthCheckSchedules
                .ToList();
        }
        public HealthCheckSchedule? GetHealthCheckScheduleById(Guid id)
        {
            return _context.HealthCheckSchedules
                .FirstOrDefault(h => h.ScheduleId == id);
        }

        public HealthCheckSchedule Add(HealthCheckSchedule schedule)
        {
            _context.HealthCheckSchedules.Add(schedule);
            _context.SaveChanges();
            return schedule;
        }

        public HealthCheckSchedule Update(HealthCheckSchedule schedule)
        {
            _context.HealthCheckSchedules.Update(schedule);
            _context.SaveChanges();
            return schedule;
        }

        public void Delete(Guid id)
        {
            var schedule = _context.HealthCheckSchedules.Find(id);
            if (schedule != null)
            {
                _context.HealthCheckSchedules.Remove(schedule);
                _context.SaveChanges();
            }
        }
    }
}
