﻿using System;
using System.Collections.Generic;

namespace SchoolMedicalWpf.Dal.Entities;

public partial class Student
{
    public Guid StudentId { get; set; }

    public Guid? UserId { get; set; }

    public string? StudentCode { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly? DayOfBirth { get; set; }

    public string? Gender { get; set; }

    public string? Grade { get; set; }

    public string? Address { get; set; }

    public string? ParentPhoneNumber { get; set; }

    public string? ParentEmailAddress { get; set; }

    public virtual ICollection<HealthCheckSchedule> HealthCheckSchedules { get; set; } = new List<HealthCheckSchedule>();

    public virtual ICollection<HealthProfile> HealthProfiles { get; set; } = new List<HealthProfile>();

    public virtual ICollection<MedicalEvent> MedicalEvents { get; set; } = new List<MedicalEvent>();

    public virtual ICollection<MedicalRegistration> MedicalRegistrations { get; set; } = new List<MedicalRegistration>();

    public virtual ICollection<VaccinationSchedule> VaccinationSchedules { get; set; } = new List<VaccinationSchedule>();
}
