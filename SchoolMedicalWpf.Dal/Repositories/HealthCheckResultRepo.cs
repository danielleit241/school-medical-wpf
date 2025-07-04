using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class HealthCheckResultRepo(SchoolmedicalWpfContext _context)
    {
        public List<HealthCheckResult> GetAll()
        {
            try
            {
                return _context.HealthCheckResults
                    .AsNoTracking()
                    .Include(hcr => hcr.HealthProfile)
                    .ThenInclude(hp => hp.Student)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public HealthCheckResult? GetById(Guid id)
        {
            try
            {
                return _context.HealthCheckResults
                    .AsNoTracking()
                    .Include(hcr => hcr.HealthProfile)
                    .ThenInclude(hp => hp.Student)
                    .FirstOrDefault(h => h.ResultId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Add(HealthCheckResult healthCheckResult)
        {
            try
            {
                var existingEntry = _context.Entry(healthCheckResult);
                if (existingEntry.State != EntityState.Detached)
                {
                    existingEntry.State = EntityState.Detached;
                }

                _context.HealthCheckResults.Add(healthCheckResult);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding health check result: {ex.Message}", ex);
            }
        }

        public void Update(HealthCheckResult healthCheckResult)
        {
            try
            {
                var trackedEntity = _context.ChangeTracker.Entries<HealthCheckResult>()
                    .FirstOrDefault(e => e.Entity.ResultId == healthCheckResult.ResultId);

                if (trackedEntity != null)
                {
                    trackedEntity.State = EntityState.Detached;
                }

                var existsInDb = _context.HealthCheckResults
                    .AsNoTracking()
                    .Any(h => h.ResultId == healthCheckResult.ResultId);

                if (!existsInDb)
                {
                    throw new InvalidOperationException($"HealthCheckResult with ID {healthCheckResult.ResultId} not found in database");
                }

                _context.HealthCheckResults.Update(healthCheckResult);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _context.ChangeTracker.Clear();
                throw new InvalidOperationException(
                    "The health check result has been modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                throw new InvalidOperationException($"Error updating health check result: {ex.Message}", ex);
            }
        }

        public void Delete(Guid id)
        {
            try
            {
                var healthCheckResult = GetById(id);
                if (healthCheckResult != null)
                {
                    var trackedEntity = _context.ChangeTracker.Entries<HealthCheckResult>()
                        .FirstOrDefault(e => e.Entity.ResultId == id);

                    if (trackedEntity != null)
                    {
                        _context.HealthCheckResults.Remove(trackedEntity.Entity);
                    }
                    else
                    {
                        _context.HealthCheckResults.Attach(healthCheckResult);
                        _context.HealthCheckResults.Remove(healthCheckResult);
                    }

                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void ClearTracking()
        {
            try
            {
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
            }
        }

        public HealthCheckResult? GetByIdNoTracking(Guid id)
        {
            try
            {
                return _context.HealthCheckResults
                    .AsNoTracking()
                    .FirstOrDefault(h => h.ResultId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<HealthCheckResult> GetAllSimple()
        {
            try
            {
                return _context.HealthCheckResults.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}