namespace SchoolMedicalWpf.Dal.Entities;

public partial class HealthCheckResult
{
    public Guid ResultId { get; set; }

    public Guid? ScheduleId { get; set; }

    public DateOnly? DatePerformed { get; set; }

    public double? Height { get; set; }

    public double? Weight { get; set; }

    public double? VisionLeft { get; set; }

    public double? VisionRight { get; set; }

    public string? Hearing { get; set; }

    public string? Nose { get; set; }

    public string? BloodPressure { get; set; }

    public string? Notes { get; set; }

    public Guid? RecordedId { get; set; }

    public Guid HealthProfileId { get; set; }
    public bool Status { get; set; }

    public virtual HealthProfile HealthProfile { get; set; } = null!;

    public virtual HealthCheckSchedule? Schedule { get; set; }
}
