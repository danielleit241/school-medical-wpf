using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolMedicalWpf.Dal.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStatusInResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "VaccinationResult",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "HealthCheckResult",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "UserID", "Address", "AvatarUrl", "DayOfBirth", "EmailAddress", "FullName", "PasswordHash", "PhoneNumber", "RoleID", "Status" },
                values: new object[,]
                {
                    { new Guid("2a9411a6-bc8d-4d87-8e80-d0e124726b49"), null, null, null, "nurse@system.local", "System Nurse", "AQAAAAIAAYagAAAAEN8BDiWU4MpZs43eQWoGXr5Q+iybxXxmj44DkOY0QOOrT2tqcxU8k1bPoFAOJwMYHw==", "0900000003", 3, null },
                    { new Guid("4efb2b1f-70b4-4587-9cc4-de8f29793951"), null, null, null, "admin@system.local", "System Admin", "AQAAAAIAAYagAAAAEN1BYdqnVPb7zRaKOGjp7qkwg8eVt/R132qDqqi+K6C+beWGfpW8eQxlNhjRq8Qykg==", "0900000001", 1, null },
                    { new Guid("7222fcfc-d5e3-42e3-bcae-43e819b8ed2f"), null, null, null, "parent@system.local", "System Parent", "AQAAAAIAAYagAAAAECK7usNnHQBcnT/L1Mpz1MeUMyjweoh+EB/HubLCJM8xZ1C8Hbm3o7sC1xgewNP/qA==", "0900000004", 4, null },
                    { new Guid("a7a1d982-e5a0-418b-a98d-981c929eda56"), null, null, null, "manager@system.local", "System Manager", "AQAAAAIAAYagAAAAECHnprDihQeUBqRnWCMBtkWvOBZsLqKfNgmqe7GGuEaSPF0fwErcYoxqMgf1rz9j0Q==", "0900000002", 2, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("2a9411a6-bc8d-4d87-8e80-d0e124726b49"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("4efb2b1f-70b4-4587-9cc4-de8f29793951"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("7222fcfc-d5e3-42e3-bcae-43e819b8ed2f"));

            migrationBuilder.DeleteData(
                table: "User",
                keyColumn: "UserID",
                keyValue: new Guid("a7a1d982-e5a0-418b-a98d-981c929eda56"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VaccinationResult");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "HealthCheckResult");

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
    }
}
