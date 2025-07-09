namespace SchoolMedicalWpf.Dal.Entities;

public partial class User
{
    public Guid UserId { get; set; }

    public int? RoleId { get; set; }

    public string? FullName { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? EmailAddress { get; set; }

    public string? AvatarUrl { get; set; }

    public DateOnly? DayOfBirth { get; set; }

    public bool? Status { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<MedicalEvent> MedicalEvents { get; set; } = new List<MedicalEvent>();

    public virtual ICollection<MedicalRegistration> MedicalRegistrations { get; set; } = new List<MedicalRegistration>();

    public virtual Role? Role { get; set; }
}
