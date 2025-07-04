using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class VaccinationResultRepo(SchoolmedicalWpfContext _context)
    {
        public List<VaccinationResult> GetAll()
        {
            try
            {
                return _context.VaccinationResults
                    .AsNoTracking()
                    .Include(vr => vr.HealthProfile)
                    .ThenInclude(hp => hp.Student)
                    .Include(vr => vr.Schedule).ThenInclude(s => s.Vaccine)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public VaccinationResult? GetById(Guid id)
        {
            try
            {
                return _context.VaccinationResults
                    .AsNoTracking()
                    .Include(vr => vr.HealthProfile)
                    .ThenInclude(hp => hp.Student)
                    .Include(vr => vr.Schedule).ThenInclude(s => s.Vaccine)
                    .FirstOrDefault(v => v.VaccinationResultId == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Add(VaccinationResult vaccinationResult)
        {
            try
            {
                var existingEntry = _context.Entry(vaccinationResult);
                if (existingEntry.State != EntityState.Detached)
                {
                    existingEntry.State = EntityState.Detached;
                }

                _context.VaccinationResults.Add(vaccinationResult);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding vaccination result: {ex.Message}", ex);
            }
        }

        public void Update(VaccinationResult vaccinationResult)
        {
            try
            {
                var trackedEntity = _context.ChangeTracker.Entries<VaccinationResult>()
                    .FirstOrDefault(e => e.Entity.VaccinationResultId == vaccinationResult.VaccinationResultId);

                if (trackedEntity != null)
                {
                    trackedEntity.State = EntityState.Detached;
                }

                var existsInDb = _context.VaccinationResults
                    .AsNoTracking()
                    .Any(v => v.VaccinationResultId == vaccinationResult.VaccinationResultId);

                if (!existsInDb)
                {
                    throw new InvalidOperationException($"VaccinationResult with ID {vaccinationResult.VaccinationResultId} not found in database");
                }

                _context.VaccinationResults.Update(vaccinationResult);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _context.ChangeTracker.Clear();
                throw new InvalidOperationException("The vaccination result has been modified by another user. Please refresh and try again.", ex);
            }
            catch (Exception ex)
            {
                _context.ChangeTracker.Clear();
                throw new InvalidOperationException($"Error updating vaccination result: {ex.Message}", ex);
            }
        }

        public void Delete(Guid id)
        {
            try
            {
                var vaccinationResult = GetById(id);
                if (vaccinationResult != null)
                {
                    var trackedEntity = _context.ChangeTracker.Entries<VaccinationResult>()
                        .FirstOrDefault(e => e.Entity.VaccinationResultId == id);

                    if (trackedEntity != null)
                    {
                        _context.VaccinationResults.Remove(trackedEntity.Entity);
                    }
                    else
                    {
                        _context.VaccinationResults.Attach(vaccinationResult);
                        _context.VaccinationResults.Remove(vaccinationResult);
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

        public List<VaccinationResult> GetAllSimple()
        {
            try
            {
                return _context.VaccinationResults.AsNoTracking().ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}