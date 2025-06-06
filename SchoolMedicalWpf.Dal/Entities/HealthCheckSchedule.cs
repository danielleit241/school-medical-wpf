﻿using System;
using System.Collections.Generic;

namespace SchoolMedicalWpf.Dal.Entities;

public partial class HealthCheckSchedule
{
    public Guid ScheduleId { get; set; }

    public Guid? StudentId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? TargetGrade { get; set; }

    public string? HealthCheckType { get; set; }

    public virtual ICollection<HealthCheckResult> HealthCheckResults { get; set; } = new List<HealthCheckResult>();

    public virtual Student? Student { get; set; }
}
