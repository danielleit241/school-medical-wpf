using System;
using System.Collections.Generic;

namespace SchoolMedicalWpf.Dal.Entities;

public partial class MedicalEvent
{
    public Guid EventId { get; set; }

    public Guid? StudentId { get; set; }

    public Guid? StaffNurseId { get; set; }

    public string? EventType { get; set; }

    public string? EventDescription { get; set; }

    public string? Location { get; set; }

    public string? SeverityLevel { get; set; }

    public bool? ParentNotified { get; set; }

    public DateOnly? EventDate { get; set; }

    public string? Notes { get; set; }

    public virtual User? StaffNurse { get; set; }

    public virtual Student? Student { get; set; }
}
