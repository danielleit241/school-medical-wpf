using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolMedicalWpf.Dal.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesAndDefaultUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3A73EBFAA9", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                columns: table => new
                {
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StudentCode = table.Column<string>(type: "varchar(8)", unicode: false, maxLength: 8, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DayOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Grade = table.Column<string>(type: "char(12)", unicode: false, fixedLength: true, maxLength: 12, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentPhoneNumber = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: true),
                    ParentEmailAddress = table.Column<string>(type: "varchar(70)", unicode: false, maxLength: 70, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Student__32C52A7919494F58", x => x.StudentID);
                });

            migrationBuilder.CreateTable(
                name: "VaccineDetails",
                columns: table => new
                {
                    VaccineID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaccineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Disease = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VaccineType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AgeRecommendation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BatchNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpirationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DoseNumber = table.Column<int>(type: "int", nullable: true),
                    ContraindicationNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__VaccineD__45DC68E9CAA27428", x => x.VaccineID);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(11)", unicode: false, maxLength: 11, nullable: false),
                    PasswordHash = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    EmailAddress = table.Column<string>(type: "varchar(70)", unicode: false, maxLength: 70, nullable: true),
                    AvatarUrl = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    DayOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__1788CCAC642EA5AB", x => x.UserID);
                    table.ForeignKey(
                        name: "FK__User__RoleID__4D94879B",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateTable(
                name: "HealthCheckSchedule",
                columns: table => new
                {
                    ScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TargetGrade = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    HealthCheckType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HealthCh__9C8A5B69CECD15D3", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK__HealthChe__Stude__534D60F1",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                });

            migrationBuilder.CreateTable(
                name: "HealthProfile",
                columns: table => new
                {
                    HealthProfileID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ChronicDiseases = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeclarationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DrugAllergies = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FoodAllergies = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HealthPr__73C2C2B587E14CF3", x => x.HealthProfileID);
                    table.ForeignKey(
                        name: "FK__HealthPro__Stude__5070F446",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                });

            migrationBuilder.CreateTable(
                name: "VaccinationSchedule",
                columns: table => new
                {
                    ScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VaccineID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Round = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    TargetGrade = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vaccinat__9C8A5B69C32EB4D4", x => x.ScheduleID);
                    table.ForeignKey(
                        name: "FK__Vaccinati__Stude__66603565",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                    table.ForeignKey(
                        name: "FK__Vaccinati__Vacci__6754599E",
                        column: x => x.VaccineID,
                        principalTable: "VaccineDetails",
                        principalColumn: "VaccineID");
                });

            migrationBuilder.CreateTable(
                name: "MedicalEvent",
                columns: table => new
                {
                    EventID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StaffNurseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EventDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    SeverityLevel = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ParentNotified = table.Column<bool>(type: "bit", nullable: true),
                    EventDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MedicalE__7944C870D7609CCF", x => x.EventID);
                    table.ForeignKey(
                        name: "FK__MedicalEv__Staff__5AEE82B9",
                        column: x => x.StaffNurseID,
                        principalTable: "User",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__MedicalEv__Stude__59FA5E80",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                });

            migrationBuilder.CreateTable(
                name: "MedicalRegistration",
                columns: table => new
                {
                    RegistrationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DateSubmitted = table.Column<DateOnly>(type: "date", nullable: true),
                    MedicationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TotalDosages = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ParentalConsent = table.Column<bool>(type: "bit", nullable: true),
                    DateApproved = table.Column<DateOnly>(type: "date", nullable: true),
                    StaffNurseID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MedicalR__6EF58830AD58F073", x => x.RegistrationID);
                    table.ForeignKey(
                        name: "FK__MedicalRe__Stude__5DCAEF64",
                        column: x => x.StudentID,
                        principalTable: "Student",
                        principalColumn: "StudentID");
                    table.ForeignKey(
                        name: "FK__MedicalRe__UserI__5EBF139D",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SourceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SenderID = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32D1A83A45", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__UserI__619B8048",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "HealthCheckResult",
                columns: table => new
                {
                    ResultID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DatePerformed = table.Column<DateOnly>(type: "date", nullable: true),
                    Height = table.Column<double>(type: "float", nullable: true),
                    Weight = table.Column<double>(type: "float", nullable: true),
                    VisionLeft = table.Column<double>(type: "float", nullable: true),
                    VisionRight = table.Column<double>(type: "float", nullable: true),
                    Hearing = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Nose = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    BloodPressure = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RecordedID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HealthProfileID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__HealthCh__97690228D7BA8C5C", x => x.ResultID);
                    table.ForeignKey(
                        name: "FK__HealthChe__Healt__571DF1D5",
                        column: x => x.HealthProfileID,
                        principalTable: "HealthProfile",
                        principalColumn: "HealthProfileID");
                    table.ForeignKey(
                        name: "FK__HealthChe__Sched__5629CD9C",
                        column: x => x.ScheduleID,
                        principalTable: "HealthCheckSchedule",
                        principalColumn: "ScheduleID");
                });

            migrationBuilder.CreateTable(
                name: "VaccinationResult",
                columns: table => new
                {
                    VaccinationResultID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DoseNumber = table.Column<int>(type: "int", nullable: true),
                    VaccinationDate = table.Column<DateOnly>(type: "date", nullable: true),
                    InjectionSite = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImmediateReaction = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ReactionStartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    ReactionType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SeverityLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RecordedID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    HealthProfileID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vaccinat__12DE8FD94AFF55DA", x => x.VaccinationResultID);
                    table.ForeignKey(
                        name: "FK__Vaccinati__Healt__6A30C649",
                        column: x => x.HealthProfileID,
                        principalTable: "HealthProfile",
                        principalColumn: "HealthProfileID");
                    table.ForeignKey(
                        name: "FK__Vaccinati__Sched__6B24EA82",
                        column: x => x.ScheduleID,
                        principalTable: "VaccinationSchedule",
                        principalColumn: "ScheduleID");
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "RoleID", "RoleName" },
                values: new object[,]
                {
                    { 1, "admin" },
                    { 2, "manager" },
                    { 3, "nurse" },
                    { 4, "parent" }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Address", "AvatarUrl", "DayOfBirth", "EmailAddress", "FullName", "PasswordHash", "PhoneNumber", "RoleID", "Status" },
                values: new object[,]
                {
                    { new Guid("14c6ffeb-7df3-4c6b-a444-3b64683839f0"), null, null, null, "admin@system.local", "System Admin", "AQAAAAIAAYagAAAAEKQpu5cJf+JT8hUM0THsrVe5E/G43pp4QFMu3Qp+1O6Th+5iuH9ZHSxfOkyQvpT4mA==", "0900000001", 1, null },
                    { new Guid("3941ef8f-9141-469a-abf5-42dbb38e6dc8"), null, null, null, "nurse@system.local", "System Nurse", "AQAAAAIAAYagAAAAEK1yY/fJJSIoXw2E+hrXt4K1vne5KtuQywNSBA6XagQj8ohqh2lmFUmTM1MxbHb4GQ==", "0900000003", 3, null },
                    { new Guid("bb3c6cbe-0c50-4ec2-8650-9e5e07967037"), null, null, null, "parent@system.local", "System Parent", "AQAAAAIAAYagAAAAEOITkxUKTMZbjct0+sOzYgq65vYUeNpWhSyoy3s0LHqgU/ihiHnd2fLRpl2k5lckHw==", "0900000004", 4, null },
                    { new Guid("f7135d34-2ec8-4de0-8663-bbee6ae66f18"), null, null, null, "manager@system.local", "System Manager", "AQAAAAIAAYagAAAAENYi/7BRP9Vh++yWX3TbbB1qepLlglqoQHYsMjnv7aFd/jlQW1F9V/xB6qmdcY/D5Q==", "0900000002", 2, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheckResult_HealthProfileID",
                table: "HealthCheckResult",
                column: "HealthProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheckResult_ScheduleID",
                table: "HealthCheckResult",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheckSchedule_StudentID",
                table: "HealthCheckSchedule",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthProfile_StudentID",
                table: "HealthProfile",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalEvent_StaffNurseID",
                table: "MedicalEvent",
                column: "StaffNurseID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalEvent_StudentID",
                table: "MedicalEvent",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRegistration_StudentID",
                table: "MedicalRegistration",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRegistration_UserID",
                table: "MedicalRegistration",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserID",
                table: "Notification",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleID",
                table: "User",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationResult_HealthProfileID",
                table: "VaccinationResult",
                column: "HealthProfileID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationResult_ScheduleID",
                table: "VaccinationResult",
                column: "ScheduleID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSchedule_StudentID",
                table: "VaccinationSchedule",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_VaccinationSchedule_VaccineID",
                table: "VaccinationSchedule",
                column: "VaccineID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HealthCheckResult");

            migrationBuilder.DropTable(
                name: "MedicalEvent");

            migrationBuilder.DropTable(
                name: "MedicalRegistration");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "VaccinationResult");

            migrationBuilder.DropTable(
                name: "HealthCheckSchedule");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "HealthProfile");

            migrationBuilder.DropTable(
                name: "VaccinationSchedule");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Student");

            migrationBuilder.DropTable(
                name: "VaccineDetails");
        }
    }
}
