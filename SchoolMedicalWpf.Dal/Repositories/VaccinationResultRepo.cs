using Microsoft.EntityFrameworkCore;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal.Repositories
{
    public class VaccinationResultRepo(SchoolmedicalWpfContext _context)
    {
        public List<VaccinationResult> GetAllVaccinationResults()
        {
            return _context.VaccinationResults
                .Include(vr => vr.HealthProfile).ThenInclude(s => s.Student)
                .Include(vr => vr.Schedule)
                .ThenInclude(s => s.Vaccine)
                .ToList();
        }
        public VaccinationResult? GetVaccinationResultById(Guid id)
        {
            return _context.VaccinationResults
                .Include(vr => vr.HealthProfile).ThenInclude(s => s.Student)
                .Include(vr => vr.Schedule)
                .ThenInclude(s => s.Vaccine)
                .FirstOrDefault(vr => vr.VaccinationResultId == id);
        }

        public void AddVaccinationResult(VaccinationResult vaccinationResult)
        {
            _context.VaccinationResults.Add(vaccinationResult);
            _context.SaveChanges();
        }
        public void UpdateVaccinationResult(VaccinationResult vaccinationResult)
        {
            _context.VaccinationResults.Update(vaccinationResult);
            _context.SaveChanges();
        }
        public void DeleteVaccinationResult(Guid id)
        {
            var vaccinationResult = _context.VaccinationResults.Find(id);
            if (vaccinationResult != null)
            {
                _context.VaccinationResults.Remove(vaccinationResult);
                _context.SaveChanges();
            }
        }
    }
}
