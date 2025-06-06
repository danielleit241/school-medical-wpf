using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SchoolMedicalWpf.Dal.Entities;

namespace SchoolMedicalWpf.Dal;

public partial class SchoolmedicalWpfContext : DbContext
{
    public SchoolmedicalWpfContext()
    {
    }

    public SchoolmedicalWpfContext(DbContextOptions<SchoolmedicalWpfContext> options)
        : base(options)
    {
    }

    public virtual DbSet<HealthCheckResult> HealthCheckResults { get; set; }

    public virtual DbSet<HealthCheckSchedule> HealthCheckSchedules { get; set; }

    public virtual DbSet<HealthProfile> HealthProfiles { get; set; }

    public virtual DbSet<MedicalEvent> MedicalEvents { get; set; }

    public virtual DbSet<MedicalRegistration> MedicalRegistrations { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VaccinationResult> VaccinationResults { get; set; }

    public virtual DbSet<VaccinationSchedule> VaccinationSchedules { get; set; }

    public virtual DbSet<VaccineDetail> VaccineDetails { get; set; }

    private string? GetConnectionString()
    {
        IConfiguration configuration = new ConfigurationBuilder()
                       .SetBasePath(Directory.GetCurrentDirectory())
                       .AddJsonFile("appsettings.json", true, true).Build();
        return configuration["ConnectionStrings:DBDefault"];
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HealthCheckResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__HealthCh__97690228D7BA8C5C");

            entity.ToTable("HealthCheckResult");

            entity.Property(e => e.ResultId)
                .ValueGeneratedNever()
                .HasColumnName("ResultID");
            entity.Property(e => e.BloodPressure).HasMaxLength(50);
            entity.Property(e => e.HealthProfileId).HasColumnName("HealthProfileID");
            entity.Property(e => e.Hearing).HasMaxLength(50);
            entity.Property(e => e.Nose).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.RecordedId).HasColumnName("RecordedID");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");

            entity.HasOne(d => d.HealthProfile).WithMany(p => p.HealthCheckResults)
                .HasForeignKey(d => d.HealthProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__HealthChe__Healt__571DF1D5");

            entity.HasOne(d => d.Schedule).WithMany(p => p.HealthCheckResults)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__HealthChe__Sched__5629CD9C");
        });

        modelBuilder.Entity<HealthCheckSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__HealthCh__9C8A5B69CECD15D3");

            entity.ToTable("HealthCheckSchedule");

            entity.Property(e => e.ScheduleId)
                .ValueGeneratedNever()
                .HasColumnName("ScheduleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.HealthCheckType).HasMaxLength(30);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TargetGrade).HasMaxLength(12);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Student).WithMany(p => p.HealthCheckSchedules)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__HealthChe__Stude__534D60F1");
        });

        modelBuilder.Entity<HealthProfile>(entity =>
        {
            entity.HasKey(e => e.HealthProfileId).HasName("PK__HealthPr__73C2C2B587E14CF3");

            entity.ToTable("HealthProfile");

            entity.Property(e => e.HealthProfileId)
                .ValueGeneratedNever()
                .HasColumnName("HealthProfileID");
            entity.Property(e => e.ChronicDiseases).HasMaxLength(255);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DrugAllergies).HasMaxLength(255);
            entity.Property(e => e.FoodAllergies).HasMaxLength(255);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Student).WithMany(p => p.HealthProfiles)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__HealthPro__Stude__5070F446");
        });

        modelBuilder.Entity<MedicalEvent>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__MedicalE__7944C870D7609CCF");

            entity.ToTable("MedicalEvent");

            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnName("EventID");
            entity.Property(e => e.EventDescription).HasMaxLength(255);
            entity.Property(e => e.EventType).HasMaxLength(30);
            entity.Property(e => e.Location).HasMaxLength(60);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.SeverityLevel).HasMaxLength(30);
            entity.Property(e => e.StaffNurseId).HasColumnName("StaffNurseID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.StaffNurse).WithMany(p => p.MedicalEvents)
                .HasForeignKey(d => d.StaffNurseId)
                .HasConstraintName("FK__MedicalEv__Staff__5AEE82B9");

            entity.HasOne(d => d.Student).WithMany(p => p.MedicalEvents)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__MedicalEv__Stude__59FA5E80");
        });

        modelBuilder.Entity<MedicalRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId).HasName("PK__MedicalR__6EF58830AD58F073");

            entity.ToTable("MedicalRegistration");

            entity.Property(e => e.RegistrationId)
                .ValueGeneratedNever()
                .HasColumnName("RegistrationID");
            entity.Property(e => e.MedicationName).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.StaffNurseId).HasColumnName("StaffNurseID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TotalDosages).HasMaxLength(30);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Student).WithMany(p => p.MedicalRegistrations)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__MedicalRe__Stude__5DCAEF64");

            entity.HasOne(d => d.User).WithMany(p => p.MedicalRegistrations)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__MedicalRe__UserI__5EBF139D");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32D1A83A45");

            entity.ToTable("Notification");

            entity.Property(e => e.NotificationId)
                .ValueGeneratedNever()
                .HasColumnName("NotificationID");
            entity.Property(e => e.ConfirmedAt).HasColumnType("datetime");
            entity.Property(e => e.SendDate).HasColumnType("datetime");
            entity.Property(e => e.SenderId).HasColumnName("SenderID");
            entity.Property(e => e.SourceId).HasColumnName("SourceID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__619B8048");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE3A73EBFAA9");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__32C52A7919494F58");

            entity.ToTable("Student");

            entity.Property(e => e.StudentId)
                .ValueGeneratedNever()
                .HasColumnName("StudentID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(3);
            entity.Property(e => e.Grade)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.ParentEmailAddress)
                .HasMaxLength(70)
                .IsUnicode(false);
            entity.Property(e => e.ParentPhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.StudentCode)
                .HasMaxLength(8)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC642EA5AB");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(70)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__User__RoleID__4D94879B");
        });

        modelBuilder.Entity<VaccinationResult>(entity =>
        {
            entity.HasKey(e => e.VaccinationResultId).HasName("PK__Vaccinat__12DE8FD94AFF55DA");

            entity.ToTable("VaccinationResult");

            entity.Property(e => e.VaccinationResultId)
                .ValueGeneratedNever()
                .HasColumnName("VaccinationResultID");
            entity.Property(e => e.HealthProfileId).HasColumnName("HealthProfileID");
            entity.Property(e => e.ImmediateReaction).HasMaxLength(100);
            entity.Property(e => e.InjectionSite).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(255);
            entity.Property(e => e.ReactionStartTime).HasColumnType("datetime");
            entity.Property(e => e.ReactionType).HasMaxLength(100);
            entity.Property(e => e.RecordedId).HasColumnName("RecordedID");
            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.SeverityLevel).HasMaxLength(50);

            entity.HasOne(d => d.HealthProfile).WithMany(p => p.VaccinationResults)
                .HasForeignKey(d => d.HealthProfileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Vaccinati__Healt__6A30C649");

            entity.HasOne(d => d.Schedule).WithMany(p => p.VaccinationResults)
                .HasForeignKey(d => d.ScheduleId)
                .HasConstraintName("FK__Vaccinati__Sched__6B24EA82");
        });

        modelBuilder.Entity<VaccinationSchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Vaccinat__9C8A5B69C32EB4D4");

            entity.ToTable("VaccinationSchedule");

            entity.Property(e => e.ScheduleId)
                .ValueGeneratedNever()
                .HasColumnName("ScheduleID");
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Round).HasMaxLength(10);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TargetGrade).HasMaxLength(12);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.VaccineId).HasColumnName("VaccineID");

            entity.HasOne(d => d.Student).WithMany(p => p.VaccinationSchedules)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Vaccinati__Stude__66603565");

            entity.HasOne(d => d.Vaccine).WithMany(p => p.VaccinationSchedules)
                .HasForeignKey(d => d.VaccineId)
                .HasConstraintName("FK__Vaccinati__Vacci__6754599E");
        });

        modelBuilder.Entity<VaccineDetail>(entity =>
        {
            entity.HasKey(e => e.VaccineId).HasName("PK__VaccineD__45DC68E9CAA27428");

            entity.Property(e => e.VaccineId)
                .ValueGeneratedNever()
                .HasColumnName("VaccineID");
            entity.Property(e => e.AgeRecommendation).HasMaxLength(50);
            entity.Property(e => e.BatchNumber).HasMaxLength(50);
            entity.Property(e => e.ContraindicationNotes).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Disease).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.VaccineName).HasMaxLength(100);
            entity.Property(e => e.VaccineType).HasMaxLength(50);
        });

        modelBuilder.Entity<Role>().HasData(
        new Role { RoleId = 1, RoleName = "admin" },
        new Role { RoleId = 2, RoleName = "manager" },
        new Role { RoleId = 3, RoleName = "nurse" },
        new Role { RoleId = 4, RoleName = "parent" }
    );

        // Chuẩn bị hash password
        var ph = new PasswordHasher<User>();

        // Seed Users (tài khoản mặc định)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                UserId = Guid.NewGuid(),
                FullName = "System Admin",
                PhoneNumber = "0900000001",
                EmailAddress = "admin@system.local",
                PasswordHash = ph.HashPassword(null, "admin@123"),
                RoleId = 1
            },
            new User
            {
                UserId = Guid.NewGuid(),
                FullName = "System Manager",
                PhoneNumber = "0900000002",
                EmailAddress = "manager@system.local",
                PasswordHash = ph.HashPassword(null, "manager@123"),
                RoleId = 2
            },
            new User
            {
                UserId = Guid.NewGuid(),
                FullName = "System Nurse",
                PhoneNumber = "0900000003",
                EmailAddress = "nurse@system.local",
                PasswordHash = ph.HashPassword(null, "nurse@123"),
                RoleId = 3
            },
            new User
            {
                UserId = Guid.NewGuid(),
                FullName = "System Parent",
                PhoneNumber = "0900000004",
                EmailAddress = "parent@system.local",
                PasswordHash = ph.HashPassword(null, "parent@123"),
                RoleId = 4
            }
        );

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
