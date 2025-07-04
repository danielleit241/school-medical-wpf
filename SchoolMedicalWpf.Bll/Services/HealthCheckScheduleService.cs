using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class HealthCheckScheduleService
    {
        private readonly HealthCheckScheduleRepo _healthCheckScheduleRepository;
        public HealthCheckScheduleService(HealthCheckScheduleRepo healthCheckScheduleRepository)
        {
            _healthCheckScheduleRepository = healthCheckScheduleRepository;
        }
        public List<HealthCheckSchedule> GetAllHealthCheckSchedules()
        {
            return _healthCheckScheduleRepository.GetAllHealthCheckSchedules();
        }
        public HealthCheckSchedule? GetHealthCheckScheduleById(Guid id)
        {
            return _healthCheckScheduleRepository.GetHealthCheckScheduleById(id);
        }
        public HealthCheckSchedule Add(HealthCheckSchedule schedule)
        {
            return _healthCheckScheduleRepository.Add(schedule);
        }
        public HealthCheckSchedule Update(HealthCheckSchedule schedule)
        {
            return _healthCheckScheduleRepository.Update(schedule);
        }
        public void Delete(Guid id)
        {
            _healthCheckScheduleRepository.Delete(id);
        }
    }
}
