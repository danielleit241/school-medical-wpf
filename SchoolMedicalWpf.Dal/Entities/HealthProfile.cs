using System;
using System.Collections.Generic;

namespace SchoolMedicalWpf.Dal.Entities;

public partial class HealthProfile
{
    public Guid HealthProfileId { get; set; }

    public Guid? StudentId { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? Notes { get; set; }

    public string? ChronicDiseases { get; set; }

    public DateOnly? DeclarationDate { get; set; }

    public string? DrugAllergies { get; set; }

    public string? FoodAllergies { get; set; }

    public virtual ICollection<HealthCheckResult> HealthCheckResults { get; set; } = new List<HealthCheckResult>();

    public virtual Student? Student { get; set; }

    public virtual ICollection<VaccinationResult> VaccinationResults { get; set; } = new List<VaccinationResult>();
}
