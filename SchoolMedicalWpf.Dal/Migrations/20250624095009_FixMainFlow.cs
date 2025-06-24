using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolMedicalWpf.Dal.Migrations
{
    /// <inheritdoc />
    public partial class FixMainFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__HealthChe__Stude__534D60F1",
                table: "HealthCheckSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK__Vaccinati__Stude__66603565",
                table: "VaccinationSchedule");

            migrationBuilder.DropIndex(
                name: "IX_VaccinationSchedule_StudentID",
                table: "VaccinationSchedule");

            migrationBuilder.DropIndex(
                name: "IX_HealthCheckSchedule_StudentID",
                table: "HealthCheckSchedule");

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("14c6ffeb-7df3-4c6b-a444-3b64683839f0"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("3941ef8f-9141-469a-abf5-42dbb38e6dc8"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("bb3c6cbe-0c50-4ec2-8650-9e5e07967037"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("f7135d34-2ec8-4de0-8663-bbee6ae66f18"));

            migrationBuilder.DropColumn(
                name: "StudentID",
                table: "VaccinationSchedule");

            migrationBuilder.DropColumn(
                name: "StudentID",
                table: "HealthCheckSchedule");

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Address", "AvatarUrl", "DayOfBirth", "EmailAddress", "FullName", "PasswordHash", "PhoneNumber", "RoleID", "Status" },
                values: new object[,]
                {
                    { new Guid("307f3796-21ed-4586-9fb5-a96f7cd0560a"), null, null, null, "manager@system.local", "System Manager", "AQAAAAIAAYagAAAAEKpjxaK4aLvqIZzQy2+5m2jqvADjBOBDbhROAL2k9N1FtJLKjj5X178TFR4ACUx3jw==", "0900000002", 2, null },
                    { new Guid("53e6b291-c871-45c9-957c-7b908711f374"), null, null, null, "nurse@system.local", "System Nurse", "AQAAAAIAAYagAAAAEABZwqvkPvaU2rMsBdd4DyvKn8bpbAI3vq7NU9a5t75vG7QH8hennmbxDZWg6LetrA==", "0900000003", 3, null },
                    { new Guid("6a61aaef-72e9-4498-b703-2b425f165675"), null, null, null, "parent@system.local", "System Parent", "AQAAAAIAAYagAAAAEAV0NGzjR0EzXScjgcB+qbMi+wQSILDYg1H4zaGj37db6wRrBjFxGt9HjkuapO4pHA==", "0900000004", 4, null },
                    { new Guid("9443b015-2ac4-4f42-a15a-4053027a1007"), null, null, null, "admin@system.local", "System Admin", "AQAAAAIAAYagAAAAEHX0YggcPr2/mztt4lSgmpMh8F9vvfFHnC+eMYEPCJmZzJXdmtD/Qlsh3TDiOwuYfw==", "0900000001", 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("307f3796-21ed-4586-9fb5-a96f7cd0560a"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("53e6b291-c871-45c9-957c-7b908711f374"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("6a61aaef-72e9-4498-b703-2b425f165675"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("9443b015-2ac4-4f42-a15a-4053027a1007"));

            migrationBuilder.AddColumn<Guid>(
                name: "StudentID",
                table: "VaccinationSchedule",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StudentID",
                table: "HealthCheckSchedule",
                type: "uniqueidentifier",
                nullable: true);

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
                name: "IX_VaccinationSchedule_StudentID",
                table: "VaccinationSchedule",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "IX_HealthCheckSchedule_StudentID",
                table: "HealthCheckSchedule",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK__HealthChe__Stude__534D60F1",
                table: "HealthCheckSchedule",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK__Vaccinati__Stude__66603565",
                table: "VaccinationSchedule",
                column: "StudentID",
                principalTable: "Student",
                principalColumn: "StudentID");
        }
    }
}
